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
    [SerializeField] RectTransform feathersOrigin;
    [SerializeField] ParticleSystem feathersParticle;
    [SerializeField] Camera mainCam;

    [SerializeField] Transform handLeft;
    [SerializeField] Transform handLeftTarget;
    [SerializeField] Transform handLeftTarget2;
    [SerializeField] Transform handRight;
    [SerializeField] Transform handRightTarget;
    [SerializeField] Transform handRightTarget2;

    [SerializeField] TextMeshProUGUI audioText;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource buttonAudio;
    [SerializeField] AudioSource itemAudio;
    [SerializeField] Thought thought;

    [SerializeField] Color[] camColors;
    [SerializeField] ParticleSystem moodParticles;
    [SerializeField] Image hoverImage;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject checker;
    int camColorIndex;

    internal bool isGameStarted;
    private float gameTimerTime;
    private bool isGameEnded;
    int choiceStage;
    private int firstChoiceIndex;
    private string choiceFirstWord;
    private string choiceSecondWord;
    private Vector3 handLeftStartPos;
    private Vector3 handRightStartPos;
    private bool isTutorialFinished;
    private bool isHoverCircleOn;
    private bool isFadingMusicOut;
    private bool isChoiceLocked;

    private void Start()
    {
        handLeftStartPos = handLeft.position;
        handRightStartPos = handRight.position;

        foreach (var careItem in careItems)
        {
            careItem.Init(this);
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
        if (isGameStarted)
        {
            if (!isGameEnded)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0.5f, 0.5f * Time.deltaTime);
                mainCam.backgroundColor = Color.Lerp(mainCam.backgroundColor, camColors[camColorIndex], 2f * Time.deltaTime);

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

            if (isFadingMusicOut)
            {
                musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0, 0.25f * Time.deltaTime);
            }
        }

        if (!isHoverCircleOn)
        {
            hoverImage.color = Color.Lerp(hoverImage.color, new Color(1f, 1f, 1f, 0f), 10f * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        UpdateParticles(bubblesOrigin, bubblesParticle);
        UpdateParticles(feathersOrigin, feathersParticle);
    }

    private void UpdateParticles(RectTransform origin, ParticleSystem particles)
    {
        Vector3 screenPos = origin.position;
        screenPos.z = 10f;
        Vector3 pos = mainCam.ScreenToWorldPoint(screenPos);
        particles.transform.position = pos;
    }

    private void EndGameplay()
    {
        itemAudio.Play();
        isGameEnded = true;
        duck.PrepareEndGame();
        thought.Hide();
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
            string dialogue = "Thank you for taking such good care of Duckie.\nDuckie is finally sleeping...";
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

            dialogue = "So, what do you think Duckie was missing the most?";
            choiceText.text = dialogue;

            letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            isChoiceLocked = true;
            foreach (var choiceButton in choiceButtons)
            {
                choiceButton.Show();
                yield return new WaitForSeconds(0.15f);
            }
            isChoiceLocked = false;
        }

        foreach (var careItem in careItems)
        {
            careItem.Outro();
        }
    }

    public void OnStartButton()
    {
        thought.Clear();
        itemAudio.Play();

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

            moodParticles.Play();
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
        if (isChoiceLocked)
        {
            return;
        }

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
        buttonAudio.Play();

        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        StartCoroutine(ThirdChoiceCoroutine());

        IEnumerator ThirdChoiceCoroutine()
        {
            choiceText.maxVisibleCharacters = 0;

            string dialogue = "Please remember to take good care of yourself too.";
            choiceText.text = dialogue;
            choiceText.enabled = true;

            float letterTime = 0;
            while (letterTime < dialogue.Length)
            {
                letterTime += TextSpeed * Time.deltaTime;
                choiceText.maxVisibleCharacters = (int)letterTime;
                yield return null;
            }

            float maxTime = 2f;
            while ((Vector3.Distance(handLeft.position, handLeftStartPos) > 0
                && Vector3.Distance(handRight.position, handRightStartPos) > 0)
                && maxTime > 0)
            {
                maxTime -= Time.deltaTime;
                handLeft.position = Vector3.MoveTowards(handLeft.position, handLeftStartPos, HandSpeed * Time.deltaTime);
                handRight.position = Vector3.MoveTowards(handRight.position, handRightStartPos, HandSpeed * Time.deltaTime);
                yield return null;
            }

            choiceText.rectTransform.localPosition = Vector3.zero;
            dialogue = "You matter so much.\nThank you for being here.\nYou have done enough today.";
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

            choiceButtons[3].SetText("again?");
            choiceButtons[3].Show();

            yield return new WaitForSeconds(0.5f);
            credits.SetActive(true);
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
        buttonAudio.Play();

        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        choiceButtons[0].SetText("YES");
        choiceButtons[3].SetText("NO");

        StartCoroutine(SecondChoiceCoroutine());

        IEnumerator SecondChoiceCoroutine()
        {
            float maxTimer = 2f;
            while (Vector3.Distance(handRight.position, handRightTarget.position) > 0 && maxTimer > 0)
            {
                maxTimer -= Time.deltaTime;
                handRight.position = Vector3.MoveTowards(handRight.position, handRightTarget.position, HandSpeed * Time.deltaTime);
                yield return null;
            }

            choiceText.maxVisibleCharacters = 0;
            string dialogue = $"That sounds right. But what about you?\nDo you have enough {choiceFirstWord} and {choiceSecondWord} in your life?";
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
                handLeft.transform.localScale = Vector3.Lerp(handLeft.transform.localScale, new Vector3(-1.5f, 1.5f, 1.5f), 5f * Time.deltaTime);
                handRight.transform.localScale = Vector3.Lerp(handRight.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), 5f * Time.deltaTime);

                handLeft.transform.position = Vector3.MoveTowards(handLeft.transform.position, handLeftTarget2.position, HandSpeed * Time.deltaTime);
                handRight.transform.position = Vector3.MoveTowards(handRight.transform.position, handRightTarget2.position, HandSpeed * Time.deltaTime);

                yield return null;
                timer -= Time.deltaTime;
            }

            duck.Hide();

            isChoiceLocked = true;
            choiceButtons[0].Show();
            yield return new WaitForSeconds(0.15f);
            choiceButtons[3].Show();
            isChoiceLocked = false;
        }
    }

    private void FirstChoice()
    {
        buttonAudio.Play();
        foreach (var choiceButton in choiceButtons)
        {
            choiceButton.Hide();
        }

        StartCoroutine(FirstChoiceCoroutine());

        IEnumerator FirstChoiceCoroutine()
        {
            float maxTimer = 2f;
            while (Vector3.Distance(handLeft.position, handLeftTarget.position) > 0 && maxTimer > 0)
            {
                maxTimer -= Time.deltaTime;
                handLeft.position = Vector3.MoveTowards(handLeft.position, handLeftTarget.position, HandSpeed * Time.deltaTime);
                yield return null;
            }

            choiceText.maxVisibleCharacters = 0;
            string dialogue = "I see... That really makes sense.\nWhat else did Duckie want?";
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

            isChoiceLocked = true;
            for (int i = 0; i < 4; i++)
            {
                if (i == firstChoiceIndex)
                    continue;
                choiceButtons[i].Show();
                yield return new WaitForSeconds(0.15f);
            }
            isChoiceLocked = false;
        }
    }

    internal void FinishTutorial()
    {
        isTutorialFinished = true;
        musicSource.Play();
        fade.gameObject.SetActive(true);
        fakeFade.SetActive(false);
        gameTimer.Show();

        StartCoroutine(GameIntroCoroutine());
        IEnumerator GameIntroCoroutine()
        {
            titleText.gameObject.SetActive(false);
            checker.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            OnStartButton();
            yield return new WaitForSeconds(40f);
            isFadingMusicOut = true;
        }
    }

    public void OnMusicClick()
    {
        buttonAudio.Play();

        if (!isTutorialFinished)
            return;

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

    internal void NextLevel()
    {
        camColorIndex++;
        camColorIndex %= camColors.Length;
    }

    internal void CheckDragProximity(Vector3 position)
    {
        float distance = Vector3.Distance(duck.transform.position, position);
        isHoverCircleOn = distance < 50f;
        if (isHoverCircleOn)
        {
            hoverImage.color = new Color(1f, 1f, 1f, 0.1f);
        }
    }

    internal void EndDrag()
    {
        isHoverCircleOn = false;
    }
}
