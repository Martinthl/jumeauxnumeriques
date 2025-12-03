using UnityEngine;
using CesiumForUnity; 
using Unity.Mathematics; // <--- AJOUT IMPORTANT ICI

public class TurbineSpawner : MonoBehaviour
{
    public GameObject TurbinePrefab;
    public TurbineMapData mapData;
    public Transform cesiumGeoreference; 

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        // Sécurité : on vérifie qu'on a bien les données
        if (mapData == null || TurbinePrefab == null)
        {
            Debug.LogError("Il manque le Prefab ou les Données dans le Spawner !");
            return;
        }

        foreach (var data in mapData.turbinePositions)
        {
            // 1. Créer l'objet
            GameObject obj = Instantiate(TurbinePrefab, cesiumGeoreference);
            obj.name = "Turbine_" + data.id;

            // 2. Le positionner sur le globe
            CesiumGlobeAnchor anchor = obj.GetComponent<CesiumGlobeAnchor>();
            
            if (anchor != null)
            {
                // CORRECTION ICI : On utilise la propriété direct avec double3
                // L'ordre est bien : Longitude (X), Latitude (Y), Hauteur (Z)
                anchor.longitudeLatitudeHeight = new double3(data.longitude, data.latitude, 0);
            }
            else
            {
                Debug.LogWarning("Attention : Le prefab n'a pas le composant CesiumGlobeAnchor !");
            }

            // 3. L'orienter
            // On applique la rotation locale (Y) pour l'orientation de l'éolienne
            obj.transform.localRotation = Quaternion.Euler(0, data.orientation, 0);
        }
    }
}