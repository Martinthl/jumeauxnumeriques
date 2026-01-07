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
    
    // (J'ai supprimé la partie Preview 3D ici)

    [Header("Navigation 3D")]
    public Transform cameraTransform;       
    public WindowGraph powerGraph; 

    private string currentSelectedID = "";

    void Start()
    {
        GenerateList();
    }

    void Update()
    {
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
        
        // Déplacement de la caméra
        FocusCameraOnTurbine(id);

        // Mise à jour du Graphique
        if (powerGraph != null)
        {
            var data = statsData.turbines.Find(t => t.turbineID == id);
            if (data != null)
            {
                List<float> powerValues = new List<float>();
                foreach(var entry in data.entries) powerValues.Add(entry.power);
                powerGraph.ShowGraph(powerValues);
            }
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

            // Affichage des Textes
            if(powerText) powerText.text = $"{entry.power:F2} kW";
            if(windText) windText.text = $"{entry.windSpeed:F1} m/s";
            if(tempText) tempText.text = $"{entry.ambientTemperature:F1} °C";
            if(rotorText) rotorText.text = $"{entry.rotorSpeed:F1} RPM";
            if(dateText) dateText.text = entry.timeInterval; 
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

            // Positionnement derrière l'éolienne
            Vector3 targetPosition = targetTurbine.position - (targetTurbine.forward * 80f) + (Vector3.up * 40f);
            Vector3 lookAtTarget = targetTurbine.position + (Vector3.up * 60f);

            cameraTransform.DOMove(targetTurbine.GetChild(2).transform.position, 2f).SetEase(Ease.OutCubic);
            cameraTransform.DOLookAt(lookAtTarget, 2f).SetEase(Ease.OutCubic);
        }
        else
        {
            Debug.LogWarning($"Éolienne {id} introuvable.");
        }
    }
}