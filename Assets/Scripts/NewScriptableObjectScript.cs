using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
 
[CreateAssetMenu(fileName = "TurbineData", menuName = "Data/Turbine CSV Data")]
public class TurbineCSVData : ScriptableObject
{
    [Header("Fichier CSV à importer")]
    public TextAsset csvFile;
 
    [Header("Données regroupées par turbine ID")]
    public List<TurbineDataParID> turbines = new List<TurbineDataParID>();
 
    [System.Serializable]
    public class TurbineEntry
    {
        public string timeInterval;
        public float timeDuration; // en secondes
        public int eventCode;
        public string eventDescription;
        public float windSpeed;
        public float ambientTemperature;
        public float rotorSpeed;
        public float power;
    }
 
    [System.Serializable]
    public class TurbineDataParID
    {
        public string turbineID;
        public List<TurbineEntry> entries = new List<TurbineEntry>();
    }
 
    [ContextMenu("Charger le CSV")]
    public void LoadCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("Aucun fichier CSV assigné !");
            return;
        }
 
        turbines.Clear();
 
        string[] lines = csvFile.text.Split('\n');
 
        for (int i = 1; i < lines.Length; i++) // ignorer l'en-tête
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
 
            string[] values = line.Split(',');
 
            if (values.Length < 8)
            {
                Debug.LogWarning($"⚠️ Ligne {i} ignorée : colonnes manquantes ({values.Length}/8)");
                continue;
            }
 
            string turbineID = values[0].Trim();
            TurbineEntry entry = new TurbineEntry
            {
                timeInterval = values[1].Trim(),
                timeDuration = ParseIntervalToSeconds(values[1].Trim()),
                eventCode = ParseInt(values[2]),
                eventDescription = values[3].Trim(),
                windSpeed = ParseFloat(values[4]),
                ambientTemperature = ParseFloat(values[5]),
                rotorSpeed = ParseFloat(values[6]),
                power = ParseFloat(values[7])
            };
 
            // Chercher si le turbineID existe déjà
            TurbineDataParID turbineData = turbines.Find(t => t.turbineID == turbineID);
            if (turbineData == null)
            {
                turbineData = new TurbineDataParID { turbineID = turbineID };
                turbines.Add(turbineData);
            }
 
            turbineData.entries.Add(entry);
        }
 
        Debug.Log($"✅ CSV chargé : {turbines.Count} turbines avec leurs entrées importées !");
    }
 
    private float ParseIntervalToSeconds(string interval)
    {
        try
        {
            string[] parts = interval.Split('-');
            if (parts.Length != 2) return 0f;
 
            float startSec = TimeStringToSeconds(parts[0].Trim());
            float endSec = TimeStringToSeconds(parts[1].Trim());
 
            return Mathf.Max(endSec - startSec, 0f);
        }
        catch
        {
            Debug.LogWarning($"Impossible de parser l'interval '{interval}'");
            return 0f;
        }
    }
 
    private float TimeStringToSeconds(string time)
    {
        string[] parts = time.Split(':');
        if (parts.Length == 2) // mm:ss
        {
            float minutes = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float seconds = float.Parse(parts[1], CultureInfo.InvariantCulture);
            return minutes * 60f + seconds;
        }
        else if (parts.Length == 3) // hh:mm:ss
        {
            float hours = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float minutes = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float seconds = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return hours * 3600f + minutes * 60f + seconds;
        }
        else
        {
            Debug.LogWarning($"Format de temps invalide : {time}");
            return 0f;
        }
    }
 
    private float ParseFloat(string input)
    {
        float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
        return result;
    }
 
    private int ParseInt(string input)
    {
        int.TryParse(input, out int result);
        return result;
    }
}