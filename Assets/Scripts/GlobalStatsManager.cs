using UnityEngine;
using TMPro;
using System.Collections.Generic; // Nécessaire pour les listes

public class GlobalStatsManager : MonoBehaviour
{
    [Header("UI Page Global")]
    public GameObject globalPanel;
    public TMP_Text totalPowerText;
    public TMP_Text avgWindText;
    public TMP_Text avgTempText;
    public TMP_Text avgRotorText;

    [Header("Graphique Global")]
    public WindowGraph globalGraphScript; // Le script sur le nouveau container

    [Header("Données")]
    public TurbineCSVData productionData; 

    public static GlobalStatsManager Instance;
    private bool graphGenerated = false; // Pour ne pas le redessiner 100 fois

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // On génère le graphique au lancement
        GenerateGlobalCurve();
    }

    void Update()
    {
        if (globalPanel != null && globalPanel.activeSelf)
        {
            UpdateGlobalStats();
        }
    }

    void GenerateGlobalCurve()
    {
        if (productionData == null || globalGraphScript == null) return;
        if (productionData.turbines.Count == 0) return;

        List<float> totalPowerOverTime = new List<float>();
        
        // On suppose que toutes les turbines ont le même nombre d'entrées
        int entryCount = productionData.turbines[0].entries.Count;

        // On parcourt le temps (chaque ligne du CSV)
        for (int i = 0; i < entryCount; i++)
        {
            float sumAtTimeT = 0f;
            // On additionne la puissance de toutes les turbines à cet instant i
            foreach (var turbine in productionData.turbines)
            {
                if (i < turbine.entries.Count)
                    sumAtTimeT += turbine.entries[i].power;
            }
            totalPowerOverTime.Add(sumAtTimeT);
        }

        // On dessine la courbe
        globalGraphScript.ShowGraph(totalPowerOverTime, "kW");
        graphGenerated = true;
    }

    void UpdateGlobalStats()
    {
        float currentTime = TimeManager.Instance.currentTimeInSeconds;
        float totalPower = 0f;
        float totalWind = 0f;
        float totalTemp = 0f;
        float totalRotor = 0f;
        int count = 0;

        foreach (var turbine in productionData.turbines)
        {
            int index = Mathf.FloorToInt((currentTime / 86400f) * turbine.entries.Count);
            index = Mathf.Clamp(index, 0, turbine.entries.Count - 1);
            var entry = turbine.entries[index];

            totalPower += entry.power;
            totalWind += entry.windSpeed;
            totalTemp += entry.ambientTemperature;
            totalRotor += entry.rotorSpeed;
            count++;
        }

        if (count > 0)
        {
            if(totalPowerText) totalPowerText.text = $"Puissance Totale : {totalPower:F0} kW";
            if(avgWindText) avgWindText.text = $"Vent Moyen : {(totalWind / count):F1} m/s";
            if(avgTempText) avgTempText.text = $"Température Moyenne : {(totalTemp / count):F1} °C";
            if(avgRotorText) avgRotorText.text = $"Rotor Moyen : {(totalRotor / count):F1} RPM";
        }
    }
}