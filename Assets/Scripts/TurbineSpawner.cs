using UnityEngine;
using CesiumForUnity;
using Unity.Mathematics; // Important pour Cesium

public class TurbineSpawner : MonoBehaviour
{
    [Header("Réglages")]
    public GameObject turbinePrefab;      
    public TurbineMapData mapData;     
    public Transform cesiumGeoreference; 

    // --- C'EST CETTE PARTIE QUI MANQUAIT PEUT-ÊTRE ---
    void Start()
    {
        SpawnAll();
    }
    // -------------------------------------------------

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

            // 2. Positionnement
            CesiumGlobeAnchor anchor = obj.GetComponent<CesiumGlobeAnchor>();
            if (anchor != null)
            {
                anchor.longitudeLatitudeHeight = new double3(data.longitude, data.latitude, 0);
            }

            // 3. Identification (Pour le tableau de bord)
            // On ajoute le script seulement s'il n'est pas déjà dessus
            TurbineIdentifier ident = obj.GetComponent<TurbineIdentifier>();
            if (ident == null) ident = obj.AddComponent<TurbineIdentifier>();
            
            ident.id = data.id; 

            // 4. Orientation
            obj.transform.localRotation = Quaternion.Euler(0, data.orientation, 0);
        }
        
        Debug.Log("✅ Turbines générées : " + mapData.turbinePositions.Count);
    }
}