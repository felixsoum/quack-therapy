using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const float MaxTime = 20f;
    [SerializeField] Duck duck;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject gameplayGroup;
    [SerializeField] CareItem[] careItems;
    [SerializeField] Image fillImage;
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
            if (gameTimer < 0)
            {
                gameTimer = 0;
            }

            fillImage.fillAmount = gameTimer / MaxTime;
        }
    }

    public void OnStartButton()
    {
        titleText.enabled = false;

        foreach (var item in careItems)
        {
            item.Show();
        }

        isGameStarted = true;
        gameplayGroup.SetActive(true);
        gameTimer = MaxTime;
        duck.OnGameStart();
    }

    internal void OnTimerTap()
    {
        gameTimer -= 5f;
    }
}
