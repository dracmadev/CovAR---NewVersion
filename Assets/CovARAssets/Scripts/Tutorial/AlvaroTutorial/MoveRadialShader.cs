using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveRadialShader : MonoBehaviour
{
    public float duration = 1.5f; // Temps que triga a fer una volta completa (360º)
    public Ease ease = Ease.Linear; // Linear fa que la velocitat de rotació sigui 100% constant

    private Material materialInstance;
    private Tween angleTween;

    private void Awake()
    {
        Image imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("No s'ha trobat el component Image a l'objecte.");
            return;
        }

        materialInstance = Instantiate(imageComponent.material);
        imageComponent.material = materialInstance;
    }

    private void OnEnable()
    {
        if (materialInstance == null) return;

        angleTween?.Kill();

        // 1. Posa l'angle inicial a 0
        materialInstance.SetFloat("_Angle", 0f);

        // 2. Anima de 0 a 360 i torna a començar des de 0 instantàniament (Restart = gir infinit)
        angleTween = materialInstance.DOFloat(360f, "_Angle", duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        angleTween?.Kill();
    }

    private void OnDestroy()
    {
        angleTween?.Kill();
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
} 