using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Duck : MonoBehaviour, IDropHandler
{
    [SerializeField] Image image;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image heart;
    [SerializeField] AudioSource quackPosAudio;
    [SerializeField] AudioSource quackNegAudio;
    [SerializeField] Thought thought;
    private Coroutine duckFlip;
    List<int> itemSequence = new();
    private int itemSequenceIndex;
    private bool isGameEnded;
    private bool isTutorial = true;
    private int tutorialSequence;
    HashSet<int> usedItems = new();
    private bool isGameEndingSoon;

    void Start()
    {
        GenerateSolution();
    }

    private void GenerateSolution()
    {
        itemSequence.Clear();

        List<int> itemIndex = new();
        for (int i = 0; i < 4; i++)
        {
            int position = UnityEngine.Random.Range(0, itemIndex.Count + 1);
            itemIndex.Insert(position, i);
        }

        for (int i = 0; i < 4; i++)
        {
            int repeat = 1;// UnityEngine.Random.Range(2, 5);
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
        Color heartColor = heart.color;
        heartColor.a = Mathf.Lerp(heartColor.a, 0, 5f * Time.deltaTime);
        heart.color = heartColor;

        Vector3 heartScale = heart.transform.localScale;
        heartScale = Vector3.Lerp(heartScale, Vector3.one * 2.5f, 5f * Time.deltaTime);
        heart.transform.localScale = heartScale;

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

        if (isGameEndingSoon)
        {
            SadDuck();
        }
        else if (isTutorial)
        {
            OnDropInTutorial(careItem);
        }
        else
        {
            if (itemSequence[itemSequenceIndex] == careItem.itemIndex)
            {
                HappyDuck();
                itemSequenceIndex++;
                itemSequenceIndex %= itemSequence.Count;
                if (itemSequenceIndex == 0)
                {
                    GenerateSolution();
                    gameManager.NextLevel();
                    StartCoroutine(NextLevelCoroutine());
                    IEnumerator NextLevelCoroutine()
                    {
                        yield return new WaitForSeconds(0.15f);
                        thought.Clear();
                    }
                }
            }
            else
            {
                SadDuck();
            }
        }
    }

    private void OnDropInTutorial(CareItem careItem)
    {
        if (usedItems.Contains(careItem.itemIndex))
        {
            SadDuck();
        }
        else
        {
            switch (tutorialSequence)
            {
                case 0:
                    if (usedItems.Count <= 2)
                    {
                        usedItems.Add(careItem.itemIndex);
                        SadDuck();
                    }
                    else
                    {
                        HappyDuck();
                        careItem.Hide();
                        tutorialSequence++;
                        usedItems.Clear();
                    }
                    break;
                case 1:
                    HappyDuck();
                    careItem.Hide();
                    tutorialSequence++;
                    break;
                case 2:
                    if (usedItems.Count == 0)
                    {
                        SadDuck();
                        usedItems.Add(careItem.itemIndex);
                    }
                    else
                    {
                        HappyDuck();
                        careItem.Hide();
                        usedItems.Clear();
                        tutorialSequence++;
                    }
                    break;
                case 3:
                    HappyDuck();
                    careItem.Hide();
                    tutorialSequence++;
                    gameManager.FinishTutorial();
                    break;
                default:
                    break;
            }
        }
    }

    private void SadDuck()
    {
        quackNegAudio.Play();
        transform.localScale = Vector3.one;
        FlipDuck(0.15f, 4);
    }

    private void HappyDuck()
    {
        thought.Fill();
        quackPosAudio.Play();
        heart.color = Color.white;
        heart.transform.localScale = Vector3.one;
    }

    internal void OnGameStart()
    {
        isTutorial = false;

        transform.localScale = Vector3.one;
        if (duckFlip != null)
        {
            StopCoroutine(duckFlip);
            duckFlip = null;
        }
    }

    internal void PrepareEndGame()
    {
        isGameEndingSoon = true;
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
