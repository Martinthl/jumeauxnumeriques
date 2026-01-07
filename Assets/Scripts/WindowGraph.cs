using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WindowGraph : MonoBehaviour
{
    [Header("Réglages")]
    public RectTransform graphContainer;
    public GameObject circlePrefab; 
    public Color lineColor = Color.green;
    public Color gridColor = new Color(1, 1, 1, 0.2f); // Gris transparent

    private List<GameObject> createdObjects = new List<GameObject>();

    public void ShowGraph(List<float> valueList, string unit = "kW")
    {
        // 1. Nettoyage
        foreach (GameObject obj in createdObjects) Destroy(obj);
        createdObjects.Clear();

        if (valueList == null || valueList.Count == 0) return;

        // 2. Calcul des échelles
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = 0f;
        foreach (float value in valueList) if (value > yMaximum) yMaximum = value;
        yMaximum = yMaximum * 1.2f; // Marge en haut

        // 3. Dessiner la grille de fond (Horizontal) - 5 lignes
        int separatorCount = 5;
        for (int i = 0; i <= separatorCount; i++)
        {
            float normalizedValue = (float)i / separatorCount;
            float yPos = normalizedValue * graphHeight;
            
            // Ligne
            CreateGridLine(new Vector2(0, yPos), new Vector2(graphWidth, yPos));
            
            // Texte de l'axe Y (Optionnel, demande un prefab Text)
            // CreateLabel(new Vector2(-20, yPos), Mathf.RoundToInt(normalizedValue * yMaximum) + " " + unit);
        }

        // 4. Dessiner la courbe
        GameObject lastCircle = null;
        int step = Mathf.Max(1, valueList.Count / 30); // Max 30 points pour la fluidité

        for (int i = 0; i < valueList.Count; i += step)
        {
            float xPosition = (i / (float)valueList.Count) * graphWidth;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;

            GameObject circle = CreateCircle(new Vector2(xPosition, yPosition));
            
            if (lastCircle != null)
            {
                CreateDotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition, 
                                    circle.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircle = circle;
        }
    }

    private void CreateGridLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("GridLine", typeof(Image));
        lineObj.transform.SetParent(graphContainer, false);
        lineObj.GetComponent<Image>().color = gridColor;
        RectTransform rect = lineObj.GetComponent<RectTransform>();
        
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);
        
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(distance, 1f); // Trait fin
        rect.anchoredPosition = start + dir * distance * 0.5f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        
        // On met la grille derrière les points
        lineObj.transform.SetAsFirstSibling(); 
        createdObjects.Add(lineObj);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = Instantiate(circlePrefab, graphContainer);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(8, 8); // Points un peu plus gros
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
        rectTransform.sizeDelta = new Vector2(distance, 3f); // Ligne plus visible
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        createdObjects.Add(gameObject);
    }
}