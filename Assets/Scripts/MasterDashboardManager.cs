using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CesiumForUnity; // Pour bouger la caméra
using Unity.Mathematics; // Pour les maths Cesium
using System.Collections.Generic;

public class MasterDashboardManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject buttonTemplatePrefab; // Le bouton qu'on a créé
    public Transform listContainer;         // L'objet "Content" de la ScrollView
    
    [Header("Données")]
    public TurbineMapData mapData;          // Pour avoir la liste des IDs et positions
    public TurbineCSVData statsData;        // Pour avoir les stats (Vent, Puissance)

    [Header("UI Détails")]
    public TMP_Text titleText;              // Titre à droite (ex: T98)
    public TMP_Text powerText;
    public TMP_Text windText;
    
    [Header("Navigation 3D")]
    public Transform cameraTransform;       // Ta Main Camera
    public CesiumGeoreference georeference; // Référence Cesium
    public float zoomDistance = 100.0f;     // Distance de la caméra

    public WindowGraph powerGraph; // Référence au script graphique

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

    // 1. Générer le tableau
    void GenerateList()
    {
        // Nettoyer la liste existante si besoin
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        foreach (var turbine in mapData.turbinePositions)
        {
            // Créer un bouton
            GameObject btn = Instantiate(buttonTemplatePrefab, listContainer);
            
            // Changer le texte du bouton
            btn.GetComponentInChildren<TMP_Text>().text = "Turbine " + turbine.id;

            // Ajouter l'action au clic
            btn.GetComponent<Button>().onClick.AddListener(() => OnTurbineSelected(turbine.id));
        }
    }

    // 2. Quand on clique sur une ligne du tableau
    public void OnTurbineSelected(string id)
    {
        currentSelectedID = id;
        titleText.text = "Turbine " + id;
        
        // Focus Caméra
        FocusCameraOnTurbine(id);
    }

    // 3. Mettre à jour les chiffres (comme avant)
    void UpdateStatsUI(string id)
    {
        float currentTime = TimeManager.Instance.currentTimeInSeconds;
        var data = statsData.turbines.Find(t => t.turbineID == id);

        if (data != null)
        {
            int index = Mathf.FloorToInt((currentTime / 86400f) * data.entries.Count);
            index = Mathf.Clamp(index, 0, data.entries.Count - 1);
            var entry = data.entries[index];

            powerText.text = $"Puissance : {entry.power:F2} kW";
            windText.text = $"Vent : {entry.windSpeed:F1} m/s";
        }
    }

    // 4. Déplacer la caméra (Fini la vue de dessus !)
    void FocusCameraOnTurbine(string id)
    {
        // On cherche toutes les étiquettes dans la scène
        TurbineIdentifier[] allTurbines = FindObjectsByType<TurbineIdentifier>(FindObjectsSortMode.None);

        foreach (var turbine in allTurbines)
        {
            // Si on trouve la bonne étiquette
            if (turbine.id == id)
            {
                // On récupère sa position
                Vector3 targetPos = turbine.transform.position;

                // On place la caméra : 30m plus haut, 60m en arrière
                // Ajuste le -60f si tu veux être plus près
                Vector3 offset = new Vector3(0, 30f, -60f); 
                
                cameraTransform.position = targetPos + offset;
                
                // On regarde le centre du mât (environ 60m de haut)
                cameraTransform.LookAt(targetPos + new Vector3(0, 60f, 0));
                
                return; // Trouvé, on arrête !
            }
        }
        Debug.LogWarning("Éolienne introuvable : " + id);
    }
}