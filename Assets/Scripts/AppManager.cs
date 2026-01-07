using UnityEngine;
using DG.Tweening; 

public class AppManager : MonoBehaviour
{
    [Header("Pages de l'interface")]
    public GameObject pageGlobal;   
    public GameObject pageDetails;  
    public GameObject pageAnalysis; // NOUVEAU : La 3ème page

    [Header("Gestion Caméra")]
    public Transform cameraTransform;   
    public Transform globalViewPoint;   

    public static AppManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GoToGlobalMode();
    }

    // --- MODE ACCUEIL ---
    public void GoToGlobalMode()
    {
        // On allume Global, on éteint TOUT le reste
        if(pageGlobal) pageGlobal.SetActive(true);
        if(pageDetails) pageDetails.SetActive(false);
        if(pageAnalysis) pageAnalysis.SetActive(false);

        // Retour caméra au ciel
        if (cameraTransform != null && globalViewPoint != null)
        {
            cameraTransform.DOKill();
            cameraTransform.DOMove(globalViewPoint.position, 2f).SetEase(Ease.InOutSine);
            cameraTransform.DORotate(globalViewPoint.rotation.eulerAngles, 2f).SetEase(Ease.InOutSine);
        }
    }

    // --- MODE DÉTAILS (LISTE) ---
    public void GoToDetailMode()
    {
        // On allume Détails, on éteint le reste
        if(pageGlobal) pageGlobal.SetActive(false);
        if(pageDetails) pageDetails.SetActive(true);
        if(pageAnalysis) pageAnalysis.SetActive(false);
    }

    // --- MODE ANALYSE (GRAPHIQUES) ---
    public void GoToAnalysisMode()
    {
        // On allume Analyse, on éteint le reste
        if(pageGlobal) pageGlobal.SetActive(false);
        if(pageDetails) pageDetails.SetActive(false);
        if(pageAnalysis) pageAnalysis.SetActive(true);
    }
}