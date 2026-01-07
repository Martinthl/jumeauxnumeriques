using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Nécessaire pour convertir les secondes en Heures:Minutes

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("UI")]
    public Slider timeSlider;
    public TMP_Text timeDisplayText; // Le texte 00:00:00

    [Header("Temps")]
    [Range(0, 86400)]
    public float currentTimeInSeconds = 28800; // Commence à 08:00

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.maxValue = 86400; // 24h en secondes
            timeSlider.wholeNumbers = true;
            timeSlider.value = currentTimeInSeconds;
            
            // Écouter le changement de valeur
            timeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        UpdateUI();
    }

    public void OnSliderChanged(float value)
    {
        currentTimeInSeconds = value;
        UpdateUI();
    }

    void UpdateUI()
    {
        // Calcul mathématique pour transformer 3600 secondes en "01:00:00"
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTimeInSeconds);
        string timeString = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        if (timeDisplayText != null)
        {
            timeDisplayText.text = timeString;
        }
    }
}