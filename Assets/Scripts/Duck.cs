using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Duck : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] Image image;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image heart;
    private Coroutine duckFlip;
    List<int> itemSequence = new();
    private int itemSequenceIndex;
    private bool isGameEnded;

    void Start()
    {
        FlipDuck(0.5f);
        GenerateSolution();
    }

    private void GenerateSolution()
    {
        List<int> itemIndex = new();
        for (int i = 0; i < 4; i++)
        {
            int position = UnityEngine.Random.Range(0, itemIndex.Count + 1);
            itemIndex.Insert(position, i);
        }

        for (int i = 0; i < 4; i++)
        {
            int repeat = UnityEngine.Random.Range(2, 5);
            for (int j = 0; j < repeat; j++)
            {
                itemSequence.Add(itemIndex[i]); 
            }
        }

        itemSequenceIndex = 0;
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
            heartScale = Vector3.Lerp(heartScale, Vector3.one * 2.5f, 5f * Time.deltaTime);
            heart.transform.localScale = heartScale;
        }

        if (isGameEnded)
        {
            image.color = Color.Lerp(image.color, Color.black, 2f * Time.deltaTime);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent<CareItem>(out var careItem))
            return;

        careItem.OnDuckDrop();

        if (itemSequence[itemSequenceIndex] == careItem.itemIndex)
        {
            HappyDuck();
            itemSequenceIndex++;
            itemSequenceIndex %= itemSequence.Count;
        }
        else
        {
            SadDuck();
        }
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
        if (duckFlip != null)
        {
            StopCoroutine(duckFlip);
            duckFlip = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.OnStartButton();
    }

    internal void EndGame()
    {
        isGameEnded = true;
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }
}
