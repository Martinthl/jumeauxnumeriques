using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

[CreateAssetMenu(menuName = "Data/Turbine Map Positions")]
public class TurbineMapData : ScriptableObject
{
    [System.Serializable]
    public class MapEntry
    {
        public string id;
        public double latitude;
        public double longitude;
        public float orientation;
    }

    public TextAsset csvFile; 
    public List<MapEntry> turbinePositions = new List<MapEntry>();

    [ContextMenu("Importer Positions")]
    public void ImportPositions()
    {
        if (csvFile == null) return;

        turbinePositions.Clear();
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');
            if (cols.Length >= 4)
            {
                MapEntry entry = new MapEntry();
                entry.id = cols[0];
                double.TryParse(cols[1], NumberStyles.Any, CultureInfo.InvariantCulture, out entry.latitude);
                double.TryParse(cols[2], NumberStyles.Any, CultureInfo.InvariantCulture, out entry.longitude);
                float.TryParse(cols[3], NumberStyles.Any, CultureInfo.InvariantCulture, out entry.orientation);
                
                turbinePositions.Add(entry);
            }
        }
        Debug.Log("Positions importées : " + turbinePositions.Count);
    }
}