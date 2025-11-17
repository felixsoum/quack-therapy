using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI choiceText;
    public int choiceIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameManager.OnChoice(choiceIndex);
    }

    internal void SetText(string text)
    {
        choiceText.text = text;
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }

    internal void Show()
    {
        gameObject.SetActive(true);
    }
}
