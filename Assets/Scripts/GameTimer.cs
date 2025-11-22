using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameManager gameManager;
    public Image fillImage;
    public Image backgroundFillImage;

    internal void Fill(float value)
    {
        fillImage.fillAmount = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.OnTimerTap();
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }
}
