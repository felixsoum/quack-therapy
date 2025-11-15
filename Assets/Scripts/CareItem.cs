using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] CanvasGroup canvasGroup;
    RectTransform myRectTransform;
    private Vector2 originalPosition;
    private bool isDragging;

    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        originalPosition = myRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        myRectTransform.anchoredPosition += eventData.delta / canvasScaler.transform.localScale.x;
    }

    void Update()
    {
        if (!isDragging)
        {
            myRectTransform.anchoredPosition = Vector3.Lerp(myRectTransform.anchoredPosition, originalPosition, 5f * Time.deltaTime);
        }
    }

    internal void Show()
    {
        gameObject.SetActive(true);
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        isDragging = false;
    }

    internal void OnDuckDrop()
    {

    }
}
