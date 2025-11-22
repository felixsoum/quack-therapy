using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float MaxTime = 20f;
    private const float TextSpeed = 30f;
    private const float HandSpeed = 500f;
    [SerializeField] Duck duck;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject gameplayGroup;
    [SerializeField] GameObject narrativeGroup;
    [SerializeField] CareItem[] careItems;
    [SerializeField] GameTimer gameTimer;
    [SerializeField] Image fade;
    [SerializeField] GameObject fakeFade;

    [SerializeField] TextMeshProUGUI choiceText;
    [SerializeField] ChoiceButton[] choiceButtons;
    [SerializeField] Image choice1;
    [SerializeField] Image choice2;
    [SerializeField] RectTransform bubblesOrigin;
    [SerializeField] ParticleSystem bubblesParticle;
    [SerializeField] Camera mainCam;

    [SerializeField] Transform handLeft;
    [SerializeField] Transform handLeftTarget;
    [SerializeField] Transform handLeftTarget2;
    [SerializeField] Transform handRight;
    [SerializeField] Transform handRightTarget;
    [SerializeField] Transform handRightTarget2;

    [SerializeField] TextMeshProUGUI audioText;
    [SerializeField] AudioSource musicSource;

    internal bool isGameStarted;
    private float gameTimerTime;
    private bool isGameEnded;
    int choiceStage;
    private int firstChoiceIndex;
    private string choiceFirstWord;
    private string choiceSecondWord;
    private Vector3 handLeftStartPos;
    private Vector3 handRightStartPos;

    private void Start()
    {
        handLeftStartPos = handLeft.position;
        handRightStartPos = handRight.position;

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
            gameTimerTime -= Time.deltaTime;
            if (gameTimerTime <= 0)
            {
                gameTimerTime = 0;
            }

            gameTimer.Fill(gameTimerTime / MaxTime);

            if (gameTimerTime == 0)
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
        isGameEnded = true;
        duck.PrepareEndGame();
        StartCoroutine(GameOutroCoroutine());
        IEnumerator GameOutroCoroutine()
        {
            while (gameTimer.backgroundFillImage.fillAmount > 0)
            {
                gameTimer.backgroundFillImage.fillAmount = Mathf.MoveTowards(gameTimer.backgroundFillImage.fillAmount, 0, 2f * Time.deltaTime);
                yield return null;
            }

            duck.EndGame();
            Color fadeColor = fade.color;
            while (fadeColor.a < 1f)
            {
                fadeColor.a += 0.5f * Time.deltaTime;
                fade.color = fadeColor;
                yield return null;
            }

            narrativeGroup.SetActive(true);

            choiceText.maxVisibleCharacters = 0;
            string dialogue = "Thanks for taking care of Duckie.\nIt's finally sleeping...";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

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
            gameTimerTime = MaxTime;
            duck.OnGameStart();
        }
    }

    internal void OnTimerTap()
    {
        gameTimerTime -= 5f;
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
        else if (choiceStage == 2)
        {
            ThirdChoice();
        }
        else
        {
            SceneManager.LoadScene(0);
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
            choiceText.maxVisibleCharacters = 0;

            string dialogue = $"Please take good care of yourself too.";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            while (Vector3.Distance(handLeft.position, handLeftStartPos) > 0
                && Vector3.Distance(handRight.position, handRightStartPos) > 0)
            {
                handLeft.position = Vector3.MoveTowards(handLeft.position, handLeftStartPos, HandSpeed * Time.deltaTime);
                handRight.position = Vector3.MoveTowards(handRight.position, handRightStartPos, HandSpeed * Time.deltaTime);
                yield return null;
            }

            choiceText.rectTransform.localPosition = Vector3.zero;
            dialogue = $"You matter so much.\nThank you for being here.\nYou have done enough today.";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            choiceButtons[3].SetText("REPLAY");
            choiceButtons[3].Show();
        }
    }

    private string AnswerByIndex(int choiceIndex)
    {
        switch (choiceIndex)
        {
            default:
                return "JOY";
            case 1:
                return "REST";
            case 2:
                return "PEACE";
            case 3:
                return "LOVE";
        }
    }

    private void SecondChoice()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        choiceButtons[0].SetText("YES");
        choiceButtons[3].SetText("NO");

        StartCoroutine(SecondChoiceCoroutine());

        IEnumerator SecondChoiceCoroutine()
        {
            while (Vector3.Distance(handRight.position, handRightTarget.position) > 0)
            {
                handRight.position = Vector3.MoveTowards(handRight.position, handRightTarget.position, HandSpeed * Time.deltaTime);
                yield return null;
            }

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

            float timer = 1f;
            while (timer > 0)
            {
                handLeft.transform.localScale = Vector3.Lerp(handLeft.transform.localScale, new Vector3(-1.5f, 1.5f, 1.5f), 3f * Time.deltaTime);
                handRight.transform.localScale = Vector3.Lerp(handRight.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), 3f * Time.deltaTime);

                handLeft.transform.position = Vector3.MoveTowards(handLeft.transform.position, handLeftTarget2.position, HandSpeed * Time.deltaTime);
                handRight.transform.position = Vector3.MoveTowards(handRight.transform.position, handRightTarget2.position, HandSpeed * Time.deltaTime);

                yield return null;
                timer -= Time.deltaTime;
            }

            duck.Hide();

            choiceButtons[0].Show();
            yield return new WaitForSeconds(0.15f);
            choiceButtons[3].Show();
        }
    }

    private void FirstChoice()
    {
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        StartCoroutine(FirstChoiceCoroutine());

        IEnumerator FirstChoiceCoroutine()
        {
            while (Vector3.Distance(handLeft.position, handLeftTarget.position) > 0)
            {
                handLeft.position = Vector3.MoveTowards(handLeft.position, handLeftTarget.position, HandSpeed * Time.deltaTime);
                yield return null;
            }

            choiceText.maxVisibleCharacters = 0;
            string dialogue = " I see... That makes sense.\nWhat else did Duckie want?";
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
        gameTimer.Show();

        StartCoroutine(GameIntroCoroutine());
        IEnumerator GameIntroCoroutine()
        {
            titleText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            OnStartButton();
        }
    }

    public void OnMusicClick()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            audioText.text = "muted";
        }
        else
        {
            musicSource.Play();
            audioText.text = "music";
        }
    }
}
