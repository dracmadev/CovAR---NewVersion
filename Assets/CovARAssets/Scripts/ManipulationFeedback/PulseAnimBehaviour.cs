using UnityEngine;
using DG.Tweening;

public class PulseAnimBehaviour : MonoBehaviour
{
    [Header("Configuració")]
    public float scaleMultiplier = 1.3f;   // Quant s’amplia
    public float pulseDuration = 0.5f;     // Temps que triga a créixer o encongir-se
    public float delayBetweenPulses = 1.5f; // Temps entre pulsacions

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartPulsing();
    }

    void StartPulsing()
    {
        // Fa un bucle infinit: gran → petit → espera → repeteix
        Sequence seq = DOTween.Sequence()
            .Append(transform.DOScale(originalScale * scaleMultiplier, pulseDuration).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, pulseDuration).SetEase(Ease.InQuad))
            .AppendInterval(delayBetweenPulses)
            .SetLoops(-1); // infinit
    }
}
