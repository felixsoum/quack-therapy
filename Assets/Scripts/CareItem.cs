using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private const float DragStartScale = 1.25f;
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform dragParent;
    [SerializeField] Transform outroTarget;
    [SerializeField] Image image;
    [SerializeField] ParticleSystem particles;
    [SerializeField] bool isParticlesBubbles;
    [SerializeField] AudioSource buttonAudio;
    [SerializeField] Tapioca[] tapiocas;
    [SerializeField] RectTransform armBack;
    [SerializeField] RectTransform armFront;
    [SerializeField] Sprite baseSprite;

    public int itemIndex;
    Transform originalParent;
    RectTransform myRectTransform;
    private Vector2 originalPosition;
    private bool isDragging;
    private bool isOutroing;
    private bool isPointerUpped;
    GameManager gameManager;

    internal Sprite GetSprite() => baseSprite != null ? baseSprite : image.sprite;

    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        originalParent = myRectTransform.parent;
        originalPosition = myRectTransform.anchoredPosition;
        if (particles != null && !isParticlesBubbles)
        {
            particles.Stop();
        }
    }

    void Start()
    {
        if (particles != null && isParticlesBubbles)
        {
            particles.Play();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isOutroing)
            return;

        Vector2 moveDelta = eventData.delta / canvasScaler.transform.localScale.x;
        myRectTransform.anchoredPosition += moveDelta;
        foreach (var p in tapiocas)
        {
            p.Push(moveDelta);
        }

        gameManager.CheckDragProximity(myRectTransform.position);
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
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 5f * Time.deltaTime);

        if (isPointerUpped)
        {
            if (armBack != null)
            {
                armBack.rotation = Quaternion.Lerp(armBack.rotation, Quaternion.identity, 5f * Time.deltaTime);
            }

            if (armFront != null)
            {
                armFront.rotation = Quaternion.Lerp(armFront.rotation, Quaternion.identity, 5f * Time.deltaTime);
            }
        }
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

        buttonAudio.Play();
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
        if (particles != null && !isParticlesBubbles)
        {
            particles.Stop();
        }

        gameManager.EndDrag();
    }

    internal void OnDuckDrop()
    {
        gameManager.EndDrag();
    }

    internal void Outro()
    {
        isOutroing = true;
        transform.parent = originalParent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerUpped = false;

        transform.localScale = DragStartScale * Vector3.one;
        if (armBack != null)
        {
            armBack.localEulerAngles = new Vector3(0, 0, -90f);
        }

        if (armFront != null)
        {
            armFront.localEulerAngles = new Vector3(0, 0, 90f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerUpped = true;
    }

    internal void Init(GameManager gameManager)
    {
        Hide();
        this.gameManager = gameManager;
    }
}
