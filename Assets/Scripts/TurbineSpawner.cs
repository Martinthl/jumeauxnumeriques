using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics; 

public class TurbineSpawner : MonoBehaviour
{
    [Header("Réglages")]
    public GameObject turbinePrefab;      
    public TurbineMapData mapData;     
    public Transform cesiumGeoreference; 

    [Header("Ajustement Hauteur")]
    [Tooltip("Ajoute de la hauteur pour ne pas être sous terre (ex: 20 ou 50)")]
    public double heightOffset = 0; 

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        if (mapData == null || turbinePrefab == null)
        {
            Debug.LogError("ERREUR : Il manque le Prefab ou les Données dans le TurbineSpawner !");
            return;
        }

        foreach (var data in mapData.turbinePositions)
        {
            // 1. Création
            GameObject obj = Instantiate(turbinePrefab, cesiumGeoreference);
            obj.name = "Turbine_" + data.id;

            // 2. Positionnement (Avec la correction de hauteur)
            CesiumGlobeAnchor anchor = obj.GetComponent<CesiumGlobeAnchor>();
            if (anchor != null)
            {
                // On utilise heightOffset au lieu de 0 pour la hauteur
                anchor.longitudeLatitudeHeight = new double3(data.longitude, data.latitude, heightOffset);
            }

            // 3. Identification
            TurbineIdentifier ident = obj.GetComponent<TurbineIdentifier>();
            if (ident == null) ident = obj.AddComponent<TurbineIdentifier>();
            
            ident.id = data.id; 

            // 4. Orientation
            obj.transform.localRotation = Quaternion.Euler(0, data.orientation, 0);
        }
        
        Debug.Log("✅ Turbines générées : " + mapData.turbinePositions.Count);
    }
}