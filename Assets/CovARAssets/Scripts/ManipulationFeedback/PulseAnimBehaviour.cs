using UnityEngine;
using DG.Tweening;

public class PulseAnimBehaviour : MonoBehaviour
{
    [Header("Type of Anim:")]
    [SerializeField] int _TypeOfAnim = 0; // 0: Pulse, 1: Rotation, 

    [Header("Configuració (Movment)")]
    [SerializeField] private float scaleMultiplier = 1.3f;   // Quant s’amplia
    [SerializeField] private float pulseDuration = 0.5f;     // Temps que triga a créixer o encongir-se
    [SerializeField] private float delayBetweenPulses = 1.5f; // Temps entre pulsacions

    [Header("Configuració (Rotation)")]
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        ChooseCorrectAnim();
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

    void StartRotation()
    {
        float duration = 360f / rotationSpeed;

        transform.DOLocalRotate(new Vector3(0, 0, 360), duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear) // Velocitat constant
            .SetLoops(-1, LoopType.Incremental); // Bucle infinit i incremental
    }

    void ChooseCorrectAnim()
    {
        switch(_TypeOfAnim)
        {
            case 0:
                StartPulsing();
                break;
            case 1:
                StartRotation();
                break;
            default:
                Debug.LogWarning("Tipus d'animació no reconegut.");
                break;
        }
    }


}
