using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Duck : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] Image image;
    [SerializeField] GameManager gameManager;
    private Coroutine duckFlip;

    void Start()
    {
        duckFlip = StartCoroutine(FlipCoroutine());
        IEnumerator FlipCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                var scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }

    void Update()
    {
        if (gameManager.isGameStarted)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 5f * Time.deltaTime); 
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        CareItem careItem = eventData.pointerDrag.GetComponent<CareItem>();
        if (careItem == null)
            return;

        transform.localScale = Vector3.one * 1.1f;
        careItem.OnDuckDrop();
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
