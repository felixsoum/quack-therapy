using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float MaxTime = 20f;
    [SerializeField] Duck duck;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject gameplayGroup;
    [SerializeField] GameObject narrativeGroup;
    [SerializeField] CareItem[] careItems;
    [SerializeField] Image fillImage;
    [SerializeField] Image fade;
    internal bool isGameStarted;
    private float gameTimer;

    private void Start()
    {
        foreach (var careItem in careItems)
        {
            careItem.Hide();
        }
    }

    private void Update()
    {
        if (isGameStarted)
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0)
            {
                gameTimer = 0;
            }

            fillImage.fillAmount = gameTimer / MaxTime;

            if (gameTimer == 0)
            {
                StartCoroutine(GameOutroCoroutine());
                IEnumerator GameOutroCoroutine()
                {
                    Color fadeColor = fade.color;
                    while (fadeColor.a < 1f)
                    {
                        fadeColor.a += 2f * Time.deltaTime;
                        fade.color = fadeColor;
                        yield return null;
                    }

                    narrativeGroup.SetActive(true);
                }
            }
        }
    }

    public void OnStartButton()
    {
        titleText.enabled = false;

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
                fadeColor.a -= 2f * Time.deltaTime;
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
}
