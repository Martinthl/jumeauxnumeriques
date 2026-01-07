using UnityEngine;
using TMPro;

public class GlobalStatsManager : MonoBehaviour
{
    [Header("UI Page Global")]
    public GameObject globalPanel;
    public TMP_Text totalPowerText; // Puissance Totale
    public TMP_Text avgWindText;    // Vent Moyen
    public TMP_Text avgTempText;    // Température Moyenne (NOUVEAU)
    public TMP_Text avgRotorText;   // Vitesse Rotor Moyenne (NOUVEAU)

    [Header("Autre")]
    public GameObject individualPanel; // Page Détails (pour la cacher)

    [Header("Données")]
    public TurbineCSVData productionData; 

    public static GlobalStatsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // On met à jour seulement si la page globale est visible
        if (globalPanel != null && globalPanel.activeSelf)
        {
            UpdateGlobalStats();
        }
    }

    void UpdateGlobalStats()
    {
        float currentTime = TimeManager.Instance.currentTimeInSeconds;
        
        float totalPower = 0f;
        float totalWind = 0f;
        float totalTemp = 0f;  // NOUVEAU
        float totalRotor = 0f; // NOUVEAU
        
        int count = 0;

        // On parcourt toutes les turbines
        foreach (var turbine in productionData.turbines)
        {
            // On cherche la ligne correspondant à l'heure du slider
            int index = Mathf.FloorToInt((currentTime / 86400f) * turbine.entries.Count);
            index = Mathf.Clamp(index, 0, turbine.entries.Count - 1);

            var entry = turbine.entries[index];

            totalPower += entry.power;
            totalWind += entry.windSpeed;
            totalTemp += entry.ambientTemperature; // NOUVEAU
            totalRotor += entry.rotorSpeed;        // NOUVEAU
            
            count++;
        }

        // Affichage
        if (count > 0)
        {
            if(totalPowerText) totalPowerText.text = $"Puissance Totale du Parc : {totalPower:F0} kW";
            if(avgWindText) avgWindText.text = $"Vent Moyen : {(totalWind / count):F1} m/s";
            
            // NOUVEAUX AFFICHAGES
            if(avgTempText) avgTempText.text = $"Température Moyenne : {(totalTemp / count):F1} °C";
            if(avgRotorText) avgRotorText.text = $"Vitesse Rotor Moyenne : {(totalRotor / count):F1} RPM";
        }
    }
}