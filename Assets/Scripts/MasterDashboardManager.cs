using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CesiumForUnity; 
using Unity.Mathematics; 
using System.Collections.Generic;
using DG.Tweening; 

public class MasterDashboardManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject buttonTemplatePrefab; 
    public Transform listContainer;         
    
    [Header("UI - Gestion Affichage")]
    public GameObject dataContainer; // La boîte qui contient toutes les infos
    public GameObject promptText;    // Le texte "Choisissez une éolienne"

    [Header("Données")]
    public TurbineMapData mapData;          
    public TurbineCSVData statsData;        

    [Header("UI Détails")]
    public TMP_Text titleText;              
    public TMP_Text powerText;
    public TMP_Text windText;
    public TMP_Text tempText;
    public TMP_Text rotorText;
    public TMP_Text dateText;

    [Header("Graphiques")]
    public WindowGraph graphPowerScript; 
    public WindowGraph graphWindScript;  

    [Header("Navigation 3D")]
    public Transform cameraTransform;       

    private string currentSelectedID = "";

    void Start()
    {
        GenerateList();
        ResetView(); // On cache les infos au début
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(currentSelectedID))
        {
            UpdateStatsUI(currentSelectedID);
        }
    }

    // Fonction pour remettre l'état "Pas de sélection"
    public void ResetView()
    {
        currentSelectedID = ""; // Aucune sélection
        if (dataContainer != null) dataContainer.SetActive(false); // Cacher les données
        if (promptText != null) promptText.SetActive(true);        // Afficher le message
    }

    void GenerateList()
    {
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        foreach (var turbine in mapData.turbinePositions)
        {
            GameObject btn = Instantiate(buttonTemplatePrefab, listContainer);
            btn.GetComponentInChildren<TMP_Text>().text = "Turbine " + turbine.id;
            btn.GetComponent<Button>().onClick.AddListener(() => OnTurbineSelected(turbine.id));
        }
    }

    public void OnTurbineSelected(string id)
    {
        // 1. On active l'affichage des données
        if (dataContainer != null) dataContainer.SetActive(true);
        if (promptText != null) promptText.SetActive(false);

        // 2. Logique standard
        currentSelectedID = id;
        if(titleText) titleText.text = "Turbine " + id;
        
        FocusCameraOnTurbine(id);

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

            // Tout en JAUNE pour les valeurs
            if(powerText) powerText.text = $"Puissance : <color=yellow>{entry.power:F2} kW</color>";
            if(windText) windText.text = $"Vent : <color=yellow>{entry.windSpeed:F1} m/s</color>";
            if(tempText) tempText.text = $"Température : <color=yellow>{entry.ambientTemperature:F1} °C</color>";
            if(rotorText) rotorText.text = $"Rotor : <color=yellow>{entry.rotorSpeed:F1} RPM</color>";
        
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
            // 1. On tue toute animation précédente immédiatement pour éviter les conflits
            cameraTransform.DOKill();

            // 2. Calcul ABSOLU de la position (Indépendant de la rotation du cylindre)
            // On prend la position de l'éolienne (X, Y, Z)
            // On ajoute un décalage fixe : Même X, 40m plus haut, 100m vers le SUD (Z - 100)
            // Comme ça, on est sûr d'être reculé, peu importe comment le cylindre tourne.
            Vector3 targetPosition = targetTurbine.position + new Vector3(0, 40f, -100f);
            
            // 3. Point à regarder (Le haut du mât)
            Vector3 lookAtTarget = targetTurbine.position + new Vector3(0, 60f, 0);

            // 4. CRÉATION D'UNE SÉQUENCE (C'est le secret pour le "1 clic")
            Sequence camSequence = DOTween.Sequence();
            
            // On ajoute le mouvement
            camSequence.Append(cameraTransform.DOMove(targetPosition, 2f).SetEase(Ease.OutCubic));
            
            // On joint la rotation EN MÊME TEMPS (Join)
            camSequence.Join(cameraTransform.DOLookAt(lookAtTarget, 2f).SetEase(Ease.OutCubic));

            Debug.Log($"Caméra séquence lancée vers {id}");
        }
        else
        {
            Debug.LogWarning($"Impossible de trouver l'éolienne : {id}");
        }
    }
}