using UnityEngine;

public class ArrowAnimationScript : MonoBehaviour
{
    public float distance = 20f;
    public float speed = 2f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;

        Vector2 dir = transform.right;
        rectTransform.anchoredPosition = startPosition + dir * offset;
    }
}