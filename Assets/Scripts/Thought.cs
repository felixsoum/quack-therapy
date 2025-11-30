using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thought : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI thoughtText;
    [SerializeField] Image background;
    int fillCounter;
    int max = 4;

    internal void Fill()
    {
        fillCounter++;
        string s = "";
        for (int i = 0; i < max - fillCounter; i++)
        {
            s += "?";
        }

        thoughtText.text = s;
    }

    internal void Clear()
    {
        fillCounter = 0;
        string s = "";

        while (s.Length < max)
        {
            s += "?";
        }

        thoughtText.text = s;
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }
}
