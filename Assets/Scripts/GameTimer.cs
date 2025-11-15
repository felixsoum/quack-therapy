using UnityEngine;
using UnityEngine.EventSystems;

public class GameTimer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameManager gameManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.OnTimerTap();
    }
}
