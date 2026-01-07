using UnityEngine;
using TMPro;
using System.Linq; // Indispensable pour les calculs de listes
using DG.Tweening;


public class GlobalStatsManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject globalPanel;
    public TMP_Text totalPowerText;
    public TMP_Text avgWindText;
    public GameObject individualPanel;

    [Header("Données")]
    public TurbineCSVData productionData; 
    
    public Transform cameraTransform; // Ta Main Camera
    public Transform globalViewPoint; // Un objet vide placé haut dans le ciel

    // Instance pour l'appeler depuis le bouton
    public static GlobalStatsManager Instance;

    void Awake()
    {
        Instance = this;
        if(globalPanel != null) globalPanel.SetActive(false);
    }

    void Update()
    {
        // Si le panneau global est ouvert, on met à jour en temps réel
        if (globalPanel.activeSelf)
        {
            UpdateGlobalStats();
        }
    }

    public void ToggleGlobalView()
    {
        bool isActive = !globalPanel.activeSelf;
        globalPanel.SetActive(isActive);

        // Si on ouvre le global, on ferme le panneau individuel pour faire propre
        if (isActive && individualPanel != null)
        {
            individualPanel.SetActive(false);
            // Optionnel : Réinitialiser la caméra ou dé-sélectionner l'éolienne
        }
    }

    void UpdateGlobalStats()
    {
        float currentTime = TimeManager.Instance.currentTimeInSeconds;
        float totalPower = 0f;
        float totalWind = 0f;
        int activeTurbineCount = 0;

        // On parcourt TOUTES les éoliennes du fichier CSV
        foreach (var turbine in productionData.turbines)
        {
            // On cherche la donnée à l'instant T pour cette turbine
            // (Même logique que le Dashboard individuel)
            int index = Mathf.FloorToInt((currentTime / 86400f) * turbine.entries.Count);
            index = Mathf.Clamp(index, 0, turbine.entries.Count - 1);

            var entry = turbine.entries[index];

            totalPower += entry.power;
            totalWind += entry.windSpeed;
            activeTurbineCount++;
        }

        // Affichage
        totalPowerText.text = $"Puissance Totale : {totalPower:F2} kW";
        
        if (activeTurbineCount > 0)
            avgWindText.text = $"Vent Moyen : {(totalWind / activeTurbineCount):F1} m/s";
        else
            avgWindText.text = "Vent Moyen : 0 m/s";
    }


    public void ResetCamera()
    {
        if(cameraTransform != null && globalViewPoint != null)
        {
            cameraTransform.DOKill(); // Arrête tout mouvement en cours
            
            // Vol vers le point global
            cameraTransform.DOMove(globalViewPoint.position, 2.5f).SetEase(Ease.InOutSine);
            cameraTransform.DORotate(globalViewPoint.rotation.eulerAngles, 2.5f).SetEase(Ease.InOutSine);
        }
    }
}