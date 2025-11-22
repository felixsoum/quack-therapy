using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform dragParent;
    [SerializeField] Transform outroTarget;
    [SerializeField] Image image;
    [SerializeField] ParticleSystem particles;

    public int itemIndex;
    Transform originalParent;
    RectTransform myRectTransform;
    private Vector2 originalPosition;
    private bool isDragging;
    private bool isOutroing;

    internal Sprite GetSprite() => image.sprite;

    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        originalParent = myRectTransform.parent;
        originalPosition = myRectTransform.anchoredPosition;
        if (particles != null)
        {
            particles.Stop(); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isOutroing)
            return;

        myRectTransform.anchoredPosition += eventData.delta / canvasScaler.transform.localScale.x;
    }

    void Update()
    {
        Vector3 targetPos = myRectTransform.anchoredPosition;

        if (isOutroing)
        {
            targetPos = outroTarget.position;
        }
        else if (!isDragging)
        {
            targetPos = originalPosition;
        }

        myRectTransform.anchoredPosition = Vector3.Lerp(myRectTransform.anchoredPosition, targetPos, 5f * Time.deltaTime);
    }

    internal void Show()
    {
        gameObject.transform.position = originalPosition;
        gameObject.SetActive(true);
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
    }

    internal void Hide()
    {
        transform.parent = originalParent;
        gameObject.SetActive(false);
        if (particles != null)
        {
            particles.Stop();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isOutroing)
            return;

        canvasGroup.blocksRaycasts = false;
        isDragging = true;
        transform.parent = dragParent;
        transform.SetAsLastSibling();
        if (particles != null)
        {
            particles.Play(); 
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isOutroing)
            return;

        canvasGroup.blocksRaycasts = true;
        isDragging = false;
        if (particles != null)
        {
            particles.Stop(); 
        }
    }

    internal void OnDuckDrop()
    {

    }

    internal void Outro()
    {
        isOutroing = true;
        transform.parent = originalParent;
    }
}
