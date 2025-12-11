using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("UI")]
    public Slider timeSlider;
    public TMP_Text timeDisplayText;

    [Header("Temps Actuel")]
    [Range(0, 86400)]
    public float currentTimeInSeconds = 28800; // Commence à 08:00 par défaut

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Initialiser le slider
        if (timeSlider != null)
        {
            timeSlider.maxValue = 86400;
            timeSlider.value = currentTimeInSeconds;
            // Quand on bouge le slider, on met à jour le temps
            timeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Update()
    {
        // Optionnel : Faire avancer le temps automatiquement (Play)
        // currentTimeInSeconds += Time.deltaTime * 60; // 1 seconde réelle = 1 minute jeu
        // UpdateUI();
    }

    public void OnSliderChanged(float value)
    {
        currentTimeInSeconds = value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeDisplayText != null)
        {
            // Convertir secondes en HH:mm:ss
            TimeSpan ts = TimeSpan.FromSeconds(currentTimeInSeconds);
            timeDisplayText.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}