using UnityEngine;
using DG.Tweening;

public class MovingFLuidraLogoScript : MonoBehaviour
{
    [Header("Configuració del Moviment (Flotació)")]
    [SerializeField] private float moveDistance = 15f;    // Distància que pujarà (en píxels de Canvas)
    [SerializeField] private float moveDuration = 3f;     // Temps del cicle de moviment

    [Header("Configuració de la Rotació (Balanceig)")]
    [SerializeField] private float maxRotation = 2f;      // Grats extres d'inclinació (molt subtil)
    [SerializeField] private float rotationDuration = 4.5f; // Temps del cicle de rotació

    [Header("Configuració de l'Escala")]
    [SerializeField] private float scaleMultiplier = 1.03f; // Multiplicador basat en l'escala actual
    [SerializeField] private float scaleDuration = 5.5f;    // Temps del cicle d'escala

    void Start()
    {
        // 1. MEMORITZEM ELS VALORS DE L'INSPECTOR
        Vector3 posicioInicial = transform.localPosition;
        Vector3 rotacioInicial = transform.localEulerAngles;
        Vector3 escalaInicial = transform.localScale;

        // 2. ANIMACIÓ DE MOVIMENT (Relativa a la posició inicial de l'Inspector)
        transform.DOLocalMoveY(posicioInicial.y + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 3. ANIMACIÓ DE ROTACIÓ (Suma els graus extres a la rotació de l'Inspector)
        // Comencem una mica cap enrere respecte a la seva rotació original
        transform.localEulerAngles = new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z - (maxRotation / 2f));

        // Anem fent yoyo sumant el rang de rotació extra configurat
        transform.DOLocalRotate(new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z + (maxRotation / 2f)), rotationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 4. ANIMACIÓ DE L'ESCALA (Multiplica respecte a l'escala inicial de l'Inspector)
        transform.DOScale(escalaInicial * scaleMultiplier, scaleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}