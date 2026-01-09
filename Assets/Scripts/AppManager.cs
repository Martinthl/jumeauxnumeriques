using UnityEngine;
using DG.Tweening; 

public class AppManager : MonoBehaviour
{
    [Header("Pages de l'interface")]
    public GameObject pageGlobal;   
    public GameObject pageDetails;  
    public GameObject pageAnalysis; 

    [Header("Gestion Caméra")]
    public Transform cameraTransform;   
    
    // Variables pour stocker la position de départ (Vue Globale)
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [Header("Scripts Externes")]
    public MasterDashboardManager masterScript; // Pour réinitialiser l'affichage détails

    public static AppManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. On sauvegarde la position exacte de la caméra au lancement du jeu
        if (cameraTransform != null)
        {
            initialPosition = cameraTransform.position;
            initialRotation = cameraTransform.rotation;
        }

        // 2. On lance le mode Global
        GoToGlobalMode();
    }

    // --- MODE ACCUEIL ---
    public void GoToGlobalMode()
    {
        // UI
        if(pageGlobal) pageGlobal.SetActive(true);
        if(pageDetails) pageDetails.SetActive(false);
        if(pageAnalysis) pageAnalysis.SetActive(false);

        // Caméra : Retour à la position de départ mémorisée
        if (cameraTransform != null)
        {
            cameraTransform.DOKill();
            cameraTransform.DOMove(initialPosition, 2f).SetEase(Ease.InOutSine);
            cameraTransform.DORotate(initialRotation.eulerAngles, 2f).SetEase(Ease.InOutSine);
        }
    }

    // --- MODE DÉTAILS (LISTE) ---
    public void GoToDetailMode()
    {
        if(pageGlobal) pageGlobal.SetActive(false);
        if(pageDetails) pageDetails.SetActive(true);
        if(pageAnalysis) pageAnalysis.SetActive(false);

        // AJOUT : On demande au MasterDashboard de remettre l'affichage à zéro
        // (Cache les données et réaffiche "Veuillez sélectionner...")
        if (masterScript != null)
        {
            masterScript.ResetView();
        }
    }

    // --- MODE ANALYSE ---
    public void GoToAnalysisMode()
    {
        if(pageGlobal) pageGlobal.SetActive(false);
        if(pageDetails) pageDetails.SetActive(false);
        if(pageAnalysis) pageAnalysis.SetActive(true);
    }
}