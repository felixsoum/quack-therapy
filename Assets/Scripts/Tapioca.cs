using UnityEngine;

public class Tapioca : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float maxDistance = 1.0f;
    Vector2 originalPosition;
    float idleTime;

    internal void Push(Vector2 moveDelta)
    {
        rectTransform.anchoredPosition += -0.2f * moveDelta;
        Vector2 direction = rectTransform.anchoredPosition - originalPosition;
        if (direction.magnitude >= maxDistance)
        {
            rectTransform.anchoredPosition = originalPosition + direction.normalized * maxDistance;
        }
        idleTime = 0;
    }

    private void Update()
    {
        if (idleTime >= 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originalPosition, 5f * Time.deltaTime);
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }
}
