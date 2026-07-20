using UnityEngine;
using DG.Tweening;
using System.Collections;

// Assegura't que l'objecte tingui un CanvasGroup
[RequireComponent(typeof(CanvasGroup))]
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

    // Guardem les posicions inicials
    private Vector3 posicioInicial;
    private Vector3 rotacioInicial;
    private Vector3 escalaInicial;

    // Component per controlar l'alpha
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Obtenim el CanvasGroup abans de res
        canvasGroup = GetComponent<CanvasGroup>();
    }

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
        // Ens assegurem de netejar tweenings previs d'aquest objecte (també el del CanvasGroup)
        transform.DOKill();
        canvasGroup.DOKill();

        // Ens assegurem que l'alpha estigui a 1 durant el bucle
        canvasGroup.alpha = 1f;

        // --- ALEATORIETAT PER DONAR MÉS NATURALITAT ---
        // Afegim petita variació a les durades (per exemple, +/- 15%) per trencar la simetria exacta
        float randomMoveDur = moveDuration * Random.Range(0.85f, 1.15f);
        float randomRotDur = rotationDuration * Random.Range(0.85f, 1.15f);
        float randomScaleDur = scaleDuration * Random.Range(0.85f, 1.15f);

        // Variació lleugera en les distàncies/valors
        float randomMoveDist = moveDistance * Random.Range(0.9f, 1.1f);
        float randomMaxRot = maxRotation * Random.Range(0.8f, 1.2f);

        // Animació de Moviment (Yoyo)
        transform.DOLocalMoveY(posicioInicial.y + randomMoveDist, randomMoveDur)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Animació de Rotació (Yoyo)
        transform.localEulerAngles = new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z - (randomMaxRot / 2f));
        transform.DOLocalRotate(new Vector3(rotacioInicial.x, rotacioInicial.y, rotacioInicial.z + (randomMaxRot / 2f)), randomRotDur)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Animació de l'Escala (Yoyo)
        transform.DOScale(escalaInicial * scaleMultiplier, randomScaleDur)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void BackgroundHide()
    {
        transform.DOKill();
        canvasGroup.DOKill(); // Aturem també l'animació de l'alpha

        Vector2 direccioAleatoria = Random.insideUnitCircle.normalized;
        float distanciaSortida = 8000f;
        Vector3 posicioFinal = posicioInicial + new Vector3(direccioAleatoria.x, direccioAleatoria.y, 0) * distanciaSortida;

        float duradaAleatoria = Random.Range(0.4f, 0.8f);

        // Animació de moviment cap a fora
        transform.DOLocalMove(posicioFinal, duradaAleatoria)
            .SetEase(Ease.InQuad);

        // --- NOVETAT: Desaparició de l'alpha (1 a 0) ---
        canvasGroup.DOFade(0f, duradaAleatoria)
            .SetEase(Ease.InQuad);
    }

    public void BackgroundShow()
    {
        // 1. Aturem qualsevol animació activa
        transform.DOKill();
        canvasGroup.DOKill();

        // 2. Generem posició exterior aleatòria
        Vector2 direccioAleatoria = Random.insideUnitCircle.normalized;
        float distanciaSortida = 8000f;
        Vector3 posicioForaDePantalla = posicioInicial + new Vector3(direccioAleatoria.x, direccioAleatoria.y, 0) * distanciaSortida;

        // 3. Teleport instantani i reseteig + ALFA A 0
        transform.localPosition = posicioForaDePantalla;
        transform.localScale = escalaInicial;
        transform.localEulerAngles = rotacioInicial;
        canvasGroup.alpha = 0f; // Comença invisible

        float duradaEntrada = 1f;

        // 4. Volar cap al centre
        transform.DOLocalMove(posicioInicial, duradaEntrada)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 5. En arribar, activem el bucle orgànic
                LoopAnim();
            });

        // --- NOVETAT: Aparició de l'alpha (0 a 1) ---
        canvasGroup.DOFade(1f, duradaEntrada)
            .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        transform.DOKill();
        if (canvasGroup != null) canvasGroup.DOKill();
    }
}