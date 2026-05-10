using UnityEngine;
using UnityEngine.UI;

public class LevelClearUI : MonoBehaviour
{
    public Text label;

    void Awake()
    {
        if (label) label.gameObject.SetActive(false);
    }

    public void Show(string text)
    {
        if (!label) return;
        label.text = text;
        label.gameObject.SetActive(true);
    }
}
