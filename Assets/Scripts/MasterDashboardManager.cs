using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CesiumForUnity; 
using Unity.Mathematics; 
using System.Collections.Generic;
using DG.Tweening; // Pour l'animation caméra

public class MasterDashboardManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject buttonTemplatePrefab; 
    public Transform listContainer;         
    
    [Header("Données")]
    public TurbineMapData mapData;          
    public TurbineCSVData statsData;        

    [Header("UI Détails - Base")]
    public TMP_Text titleText;              
    public TMP_Text powerText;
    public TMP_Text windText;

    [Header("UI Détails - Avancé")]
    public TMP_Text tempText;
    public TMP_Text rotorText;
    public TMP_Text dateText;

    [Header("Graphiques")]
    public WindowGraph graphPowerScript; // Glisse ton Graph_Power ici
    public WindowGraph graphWindScript;  // Glisse ton Graph_Wind ici

    [Header("Navigation 3D")]
    public Transform cameraTransform;       

    private string currentSelectedID = "";

    void Start()
    {
        GenerateList();
    }

    void Update()
    {
        // Mise à jour en temps réel des stats si une turbine est sélectionnée
        if (!string.IsNullOrEmpty(currentSelectedID))
        {
            UpdateStatsUI(currentSelectedID);
        }
    }

    void GenerateList()
    {
        // Nettoyage de la liste
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        // Création des boutons
        foreach (var turbine in mapData.turbinePositions)
        {
            GameObject btn = Instantiate(buttonTemplatePrefab, listContainer);
            btn.GetComponentInChildren<TMP_Text>().text = "Turbine " + turbine.id;
            btn.GetComponent<Button>().onClick.AddListener(() => OnTurbineSelected(turbine.id));
        }
    }

    public void OnTurbineSelected(string id)
    {
        currentSelectedID = id;
        if(titleText) titleText.text = "Turbine " + id;
        
        FocusCameraOnTurbine(id);

        // --- MISE À JOUR DES 2 GRAPHIQUES ---
        var data = statsData.turbines.Find(t => t.turbineID == id);
        if (data != null)
        {
            List<float> powerList = new List<float>();
            List<float> windList = new List<float>();

            foreach(var entry in data.entries)
            {
                powerList.Add(entry.power);
                windList.Add(entry.windSpeed);
            }

            // Envoyer les données aux scripts respectifs avec l'unité correcte
            if (graphPowerScript != null) graphPowerScript.ShowGraph(powerList, "kW");
            if (graphWindScript != null) graphWindScript.ShowGraph(windList, "m/s");
        }
    }

    void UpdateStatsUI(string id)
    {
        float currentTime = TimeManager.Instance.currentTimeInSeconds;
        var data = statsData.turbines.Find(t => t.turbineID == id);

        if (data != null)
        {
            int index = Mathf.FloorToInt((currentTime / 86400f) * data.entries.Count);
            index = Mathf.Clamp(index, 0, data.entries.Count - 1);
            var entry = data.entries[index];

            // Affichage avec Couleurs et Unités corrigées
            if(powerText) powerText.text = $"Puissance : <color=yellow>{entry.power:F2} kW</color>";
            if(windText) windText.text = $"Vent : <color=cyan>{entry.windSpeed:F1} m/s</color>"; // Corrigé : m/s
            
            if(tempText) tempText.text = $"Température : {entry.ambientTemperature:F1} °C";
            if(rotorText) rotorText.text = $"Rotor : {entry.rotorSpeed:F1} RPM";
        
            if(dateText) dateText.text = TimeManager.Instance.GetFormattedTime();
        }
    }

    void FocusCameraOnTurbine(string id)
    {
        TurbineIdentifier[] allTurbines = FindObjectsByType<TurbineIdentifier>(FindObjectsSortMode.None);
        Transform targetTurbine = null;

        foreach (var t in allTurbines)
        {
            if (t.id == id)
            {
                targetTurbine = t.transform;
                break;
            }
        }

        if (targetTurbine != null)
        {
            cameraTransform.DOKill(); 

            // Positionnement derrière l'éolienne (recul de 80m, hauteur de 40m)
            Vector3 targetPosition = targetTurbine.position - (targetTurbine.forward * 80f) + (Vector3.up * 40f);
            
            // On regarde le centre du rotor (hauteur de 60m)
            Vector3 lookAtTarget = targetTurbine.position + (Vector3.up * 60f);

            cameraTransform.DOMove(targetPosition, 2f).SetEase(Ease.OutCubic);
            cameraTransform.DOLookAt(lookAtTarget, 2f).SetEase(Ease.OutCubic);
        }
        else
        {
            Debug.LogWarning($"Éolienne {id} introuvable.");
        }
    }
}