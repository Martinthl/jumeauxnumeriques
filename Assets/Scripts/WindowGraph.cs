using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic;

public class WindowGraph : MonoBehaviour
{
    [Header("Réglages")]
    public RectTransform graphContainer;
    public GameObject circlePrefab; 
    public Color lineColor = Color.green;
    public Color gridColor = new Color(1, 1, 1, 0.2f); // Gris transparent

    [Header("Labels")]
    public GameObject labelPrefab; // Le prefab Label_Template

    private List<GameObject> createdObjects = new List<GameObject>();

    public void ShowGraph(List<float> valueList, string unit = "")
    {
        // 1. Nettoyage
        foreach (GameObject obj in createdObjects) Destroy(obj);
        createdObjects.Clear();

        if (valueList == null || valueList.Count == 0) return;

        // 2. Calculer le Max
        float graphHeight = graphContainer.rect.height;
        float graphWidth = graphContainer.rect.width;
        float yMaximum = 0f;
        foreach (float value in valueList) if (value > yMaximum) yMaximum = value;
        yMaximum = yMaximum * 1.2f; // Marge de 20% en haut

        // 3. Dessiner la grille et les Labels (5 lignes horizontales)
        int separatorCount = 5;
        for (int i = 0; i <= separatorCount; i++)
        {
            float normalizedValue = (float)i / separatorCount;
            float yPos = normalizedValue * graphHeight;
            
            // Ligne de grille
            CreateGridLine(new Vector2(0, yPos), new Vector2(graphWidth, yPos));
            
            // Label (ex: "500 kW") - On ne l'affiche pas pour 0 pour éviter la surcharge si voulu
            if (labelPrefab != null)
            {
                CreateLabel(yPos, Mathf.RoundToInt(normalizedValue * yMaximum) + " " + unit);
            }
        }

        // 4. Dessiner la courbe
        GameObject lastCircle = null;
        int step = Mathf.Max(1, valueList.Count / 30); // Limite le nombre de points pour la performance

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
        
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.sizeDelta = new Vector2(distance, 1f); // Trait fin
        rect.anchoredPosition = start + dir * distance * 0.5f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        
        lineObj.transform.SetAsFirstSibling(); // Derrière tout le reste
        createdObjects.Add(lineObj);
    }

    private void CreateLabel(float yPos, string text)
    {
        GameObject labelObj = Instantiate(labelPrefab, graphContainer);
        labelObj.transform.SetAsLastSibling();
        
        RectTransform rect = labelObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-5, yPos); // Juste à gauche du graph
        rect.pivot = new Vector2(1, 0.5f); // Pivot à droite
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        
        // --- CORRECTION DU BUG NULL REFERENCE ---
        TMP_Text tmp = labelObj.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Right;
            tmp.color = Color.white;
        }
        // ----------------------------------------

        createdObjects.Add(labelObj);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = Instantiate(circlePrefab, graphContainer);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(4, 4); 
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
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        createdObjects.Add(gameObject);
    }
}