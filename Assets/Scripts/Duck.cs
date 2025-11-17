using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Duck : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] Image image;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image heart;
    private Coroutine duckFlip;

    void Start()
    {
        FlipDuck(0.5f);
    }

    private void FlipDuck(float delay, int loop = -1)
    {
        if (duckFlip != null)
        {
            StopCoroutine(duckFlip);
        }

        duckFlip = StartCoroutine(FlipCoroutine());
        IEnumerator FlipCoroutine()
        {
            while (loop == -1 || loop-- > 0)
            {
                var scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                yield return new WaitForSeconds(delay);
            }
            duckFlip = null;
        }
    }

    void Update()
    {
        if (gameManager.isGameStarted)
        {
            Color heartColor = heart.color;
            heartColor.a = Mathf.Lerp(heartColor.a, 0, 5f * Time.deltaTime);
            heart.color = heartColor;

            Vector3 heartScale = heart.transform.localScale;
            heartScale = Vector3.Lerp(heartScale, Vector3.one * 2f, 5f * Time.deltaTime);
            heart.transform.localScale = heartScale;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        CareItem careItem = eventData.pointerDrag.GetComponent<CareItem>();
        if (careItem == null)
            return;

        careItem.OnDuckDrop();
        //HappyDuck();
        SadDuck();
    }

    private void SadDuck()
    {
        FlipDuck(0.15f, 4);
    }

    private void HappyDuck()
    {
        heart.color = Color.white;
        heart.transform.localScale = Vector3.one;
    }

    internal void OnGameStart()
    {
        transform.localScale = Vector3.one;
        StopCoroutine(duckFlip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.OnStartButton();
    }
}
