using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MovingFLuidraLogoScript : MonoBehaviour
{
    [Header("Configuració del Moviment (Flotació)")]
    [SerializeField] private float moveDistance = 15f;
    [SerializeField] private float moveDuration = 3f;

    [Header("Configuració de la Rotació (Balanceig)")]
    [SerializeField] private float maxRotation = 2f;
    [SerializeField] private float rotationDuration = 4.5f;

    [Header("Configuració de l'Escala")]
    [SerializeField] private float scaleMultiplier = 1.03f;
    [SerializeField] private float scaleDuration = 5.5f;

    // Guardem les posicions inicials de l'Inspector a nivell de classe
    private Vector3 posicioInicial;
    private Vector3 rotacioInicial;
    private Vector3 escalaInicial;

    void Start()
    {
        // 1. MEMORITZEM ELS VALORS DE L'INSPECTOR NOMÉS UNA VEGADA
        posicioInicial = transform.localPosition;
        rotacioInicial = transform.localEulerAngles;
        escalaInicial = transform.localScale;


        BackgroundShow();
    }


   

    private void LoopAnim()
    {
        // Ens assegurem de netejar tweenings previs d'aquest objecte
        transform.DOKill();

        // Animació de Moviment (Yoyo)
        transform.DOLocalMoveY(posicioInicial.y + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Animació de Rotació (Yoyo)
        transform.localEulerAngles = new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z - (maxRotation / 2f));
        transform.DOLocalRotate(new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z + (maxRotation / 2f)), rotationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Animació de l'Escala (Yoyo)
        transform.DOScale(escalaInicial * scaleMultiplier, scaleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }


    public void BackgroundHide()
    {
        transform.DOKill();

        Vector2 direccioAleatoria = Random.insideUnitCircle.normalized;
        float distanciaSortida = 8000f; // Reduït una mica de 20000f a 2000f perquè a 2000f ja passa de llarg qualsevol Canvas de sobres
        Vector3 posicioFinal = posicioInicial + new Vector3(direccioAleatoria.x, direccioAleatoria.y, 0) * distanciaSortida;

        float duradaAleatoria = Random.Range(0.4f, 0.8f);

        transform.DOLocalMove(posicioFinal, duradaAleatoria)
            .SetEase(Ease.InQuad);
    }


    public void BackgroundShow()
    {
        // 1. Aturem qualsevol animació que s'estigui executant
        transform.DOKill();

        // 2. Generem una posició inicial aleatòria a l'exterior del Canvas (el mateix càlcul que el Hide)
        Vector2 direccioAleatoria = Random.insideUnitCircle.normalized;
        float distanciaSortida = 8000f;
        Vector3 posicioForaDePantalla = posicioInicial + new Vector3(direccioAleatoria.x, direccioAleatoria.y, 0) * distanciaSortida;

        // 3. Teleportem el logotip instantàniament a fora de la pantalla i resetejem escala i rotació per seguretat
        transform.localPosition = posicioForaDePantalla;
        transform.localScale = escalaInicial;
        transform.localEulerAngles = rotacioInicial;

        // 4. Fem que torni volant a la seva posició inicial original en 0.5 segons
        // Useu Ease.OutQuad perquè deceleri de manera elegant quan arribi al centre
        transform.DOLocalMove(posicioInicial,1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 5. Quan ha arribat exactament al seu lloc inicial, empalmem amb el bucle
                LoopAnim();
            });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}