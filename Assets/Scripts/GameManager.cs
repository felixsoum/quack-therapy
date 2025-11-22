using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float MaxTime = 20f;
    private const float TextSpeed = 30f;
    [SerializeField] Duck duck;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject gameplayGroup;
    [SerializeField] GameObject narrativeGroup;
    [SerializeField] CareItem[] careItems;
    [SerializeField] GameObject timerObject;
    [SerializeField] Image fillImage;
    [SerializeField] Image fade;
    [SerializeField] GameObject fakeFade;

    [SerializeField] TextMeshProUGUI choiceText;
    [SerializeField] ChoiceButton[] choiceButtons;
    [SerializeField] Image choice1;
    [SerializeField] Image choice2;
    [SerializeField] RectTransform bubblesOrigin;
    [SerializeField] ParticleSystem bubblesParticle;
    [SerializeField] Camera mainCam;

    internal bool isGameStarted;
    private float gameTimer;
    private bool isGameEnded;
    int choiceStage;
    private int firstChoiceIndex;
    private string choiceFirstWord;
    private string choiceSecondWord;

    private void Start()
    {
        foreach (var careItem in careItems)
        {
            careItem.Hide();
        }

        StartCoroutine(ItemIntroCoroutine());
        IEnumerator ItemIntroCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var careItem in careItems)
            {
                careItem.Show();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    private void Update()
    {
        if (isGameStarted && !isGameEnded)
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0)
            {
                gameTimer = 0;
            }

            fillImage.fillAmount = gameTimer / MaxTime;

            if (gameTimer == 0)
            {
                EndGameplay();
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 screenPos = bubblesOrigin.position;
        screenPos.z = 10f;
        Vector3 pos = mainCam.ScreenToWorldPoint(screenPos);
        bubblesParticle.transform.position = pos;
    }

    private void EndGameplay()
    {
        duck.EndGame();
        isGameEnded = true;
        StartCoroutine(GameOutroCoroutine());
        IEnumerator GameOutroCoroutine()
        {
            Color fadeColor = fade.color;
            while (fadeColor.a < 1f)
            {
                fadeColor.a += 0.5f * Time.deltaTime;
                fade.color = fadeColor;
                yield return null;
            }

            narrativeGroup.SetActive(true);

            choiceText.maxVisibleCharacters = 0;
            string dialogue = "Thanks for taking care of Duckie.\nIt's asleep now...";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(3f);

            dialogue = "So what was Duckie missing the most?";
            choiceText.text = dialogue;

            letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            foreach (var choiceButton in choiceButtons)
            {
                choiceButton.Show();
                yield return new WaitForSeconds(0.15f);
            }
        }

        foreach (var careItem in careItems)
        {
            careItem.Outro();
        }
    }

    public void OnStartButton()
    {
        foreach (var item in careItems)
        {
            item.Show();
        }

        gameplayGroup.SetActive(true);

        StartCoroutine(GameIntroCoroutine());
        IEnumerator GameIntroCoroutine()
        {
            Color fadeColor = fade.color;
            while (fadeColor.a > 0)
            {
                fadeColor.a -= 0.5f * Time.deltaTime;
                fade.color = fadeColor;
                yield return null;
            }

            isGameStarted = true;
            gameTimer = MaxTime;
            duck.OnGameStart();
        }
    }

    internal void OnTimerTap()
    {
        gameTimer -= 5f;
    }

    internal void OnChoice(int choiceIndex)
    {
        if (choiceStage == 0)
        {
            firstChoiceIndex = choiceIndex;
            choiceFirstWord = AnswerByIndex(choiceIndex);
            choice1.sprite = careItems[choiceIndex].GetSprite();
            choice1.enabled = true;
            FirstChoice();
        }
        else if (choiceStage == 1)
        {
            choiceSecondWord = AnswerByIndex(choiceIndex);
            choice2.sprite = careItems[choiceIndex].GetSprite();
            choice2.enabled = true;
            SecondChoice();
        }
        else
        {
            ThirdChoice();
        }
        choiceStage++;
    }

    private void ThirdChoice()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        StartCoroutine(ThirdChoiceCoroutine());

        IEnumerator ThirdChoiceCoroutine()
        {
            duck.Hide();
            choiceText.maxVisibleCharacters = 0;
            string dialogue = $"Please take good care of yourself too.\nYou matter so much.\nThank you for being here.";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            //for (int i = 1; i < 3; i++)
            //{
            //    choiceButtons[i].Show();
            //    yield return new WaitForSeconds(0.15f);
            //}
        }
    }

    private string AnswerByIndex(int choiceIndex)
    {
        switch (choiceIndex)
        {
            default:
                return "FOOD";
            case 1:
                return "TIME";
            case 2:
                return "LOVE";
            case 3:
                return "JOY";
        }
    }

    private void SecondChoice()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        choiceButtons[1].SetText("YES");
        choiceButtons[2].SetText("NO");

        StartCoroutine(FirstChoiceCoroutine());

        IEnumerator FirstChoiceCoroutine()
        {
            choiceText.maxVisibleCharacters = 0;
            string dialogue = $"That's right... But what about you?\nDo you have enough {choiceFirstWord} and {choiceSecondWord}?";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            for (int i = 1; i < 3; i++)
            {
                choiceButtons[i].Show();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    private void FirstChoice()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        //choiceButtons[1].SetText("YES");
        //choiceButtons[2].SetText("NO");

        StartCoroutine(FirstChoiceCoroutine());

        IEnumerator FirstChoiceCoroutine()
        {
            choiceText.maxVisibleCharacters = 0;
            string dialogue = " I see... That makes sense.\nWhat about the second most important care?";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            for (int i = 0; i < 4; i++)
            {
                if (i == firstChoiceIndex)
                    continue;
                choiceButtons[i].Show();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    internal void FinishTutorial()
    {
        fade.gameObject.SetActive(true);
        fakeFade.SetActive(false);
        timerObject.SetActive(true);

        StartCoroutine(GameIntroCoroutine());
        IEnumerator GameIntroCoroutine()
        {
            titleText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            OnStartButton();
        }
    }
}
