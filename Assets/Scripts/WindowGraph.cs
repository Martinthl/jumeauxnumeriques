using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WindowGraph : MonoBehaviour
{
    [Header("Réglages")]
    public RectTransform graphContainer;
    public GameObject circlePrefab; // Ton prefab de point blanc
    public Color lineColor = Color.green;

    private List<GameObject> createdObjects = new List<GameObject>();

    // Cette fonction sera appelée par le Dashboard
    public void ShowGraph(List<float> valueList)
    {
        // 1. Nettoyer l'ancien graphique
        foreach (GameObject obj in createdObjects) Destroy(obj);
        createdObjects.Clear();

        if (valueList == null || valueList.Count == 0) return;

        // 2. Calculer les échelles
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = 0f;
        
        // Trouver la valeur max pour que le graph rentre en hauteur
        foreach (float value in valueList) {
            if (value > yMaximum) yMaximum = value;
        }
        yMaximum = yMaximum * 1.1f; // Ajouter 10% de marge en haut

        // 3. Dessiner les points et les lignes
        GameObject lastCircleGameObject = null;
        
        // On affiche max 20 points pour ne pas surcharger, ou tout si c'est peu
        int step = Mathf.Max(1, valueList.Count / 20); 

        for (int i = 0; i < valueList.Count; i += step)
        {
            float xPosition = (i / (float)valueList.Count) * graphWidth;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;

            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, 
                                    circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = Instantiate(circlePrefab, graphContainer);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5, 5); // Taille du point
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        
        createdObjects.Add(gameObject);
        return gameObject;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        
        gameObject.GetComponent<Image>().color = lineColor;
        
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 2f); // 2f = épaisseur du trait
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        
        // Calcul de l'angle pour tourner la ligne
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        
        createdObjects.Add(gameObject);
    }
}