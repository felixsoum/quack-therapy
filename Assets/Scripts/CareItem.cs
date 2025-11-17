using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform dragParent;
    Transform originalParent;
    RectTransform myRectTransform;
    private Vector2 originalPosition;
    private bool isDragging;

    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        originalParent = myRectTransform.parent;
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
        transform.parent = dragParent;
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        isDragging = false;
        //transform.parent = originalParent;
    }

    internal void OnDuckDrop()
    {

    }
}
