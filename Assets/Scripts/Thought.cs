using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thought : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI thoughtText;
    [SerializeField] Image background;
    [SerializeField] Color baseColor;
    [SerializeField] Color successColor;
    int fillCounter;

    private void Update()
    {
        background.color = Color.Lerp(background.color, baseColor, 5f * Time.deltaTime);
        background.transform.localScale = Vector3.Lerp(background.transform.localScale, Vector3.one, 5f * Time.deltaTime);
    }

    internal void Fill()
    {
        background.color = successColor;
        background.transform.localScale = Vector3.one * 1.15f;
        fillCounter++;
        thoughtText.text = fillCounter.ToString();
    }

    internal void Clear()
    {
        fillCounter = 0;
        thoughtText.text = "0";
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }
}
