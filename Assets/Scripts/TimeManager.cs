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

    [Header("Réglages Temps")]
    [Range(0, 86400)]
    public float currentTimeInSeconds = 28800; // 08:00
    
    [Tooltip("Vitesse d'écoulement du temps. 1 = Temps réel. 60 = 1 minute par seconde.")]
    public float timeSpeed = 1.0f; // Mets 10 ou 60 pour que ça avance plus vite

    public bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.maxValue = 86400;
            timeSlider.wholeNumbers = true;
            timeSlider.value = currentTimeInSeconds;
            timeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Update()
    {
        // Si on n'est pas en pause, on avance le temps
        if (!isPaused)
        {
            // On ajoute le temps écoulé * la vitesse
            currentTimeInSeconds += Time.deltaTime * timeSpeed;

            // Si on dépasse 24h (86400s), on revient à 00:00
            if (currentTimeInSeconds >= 86400)
            {
                currentTimeInSeconds = 0;
            }

            // On met à jour le slider (qui mettra à jour l'UI via l'event)
            if (timeSlider != null)
            {
                timeSlider.value = currentTimeInSeconds; 
            }
        }
        
        // Mise à jour du texte constante
        UpdateTextUI();
    }

    // Appelée quand on bouge le slider à la souris
    public void OnSliderChanged(float value)
    {
        currentTimeInSeconds = value;
        UpdateTextUI();
    }

    void UpdateTextUI()
    {
        if (timeDisplayText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTimeInSeconds);
            // Affiche format HH:mm:ss (ex: 08:05:23)
            timeDisplayText.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }
    
    // Petite fonction utilitaire pour que les autres scripts récupèrent le texte propre
    public string GetFormattedTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTimeInSeconds);
        return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}