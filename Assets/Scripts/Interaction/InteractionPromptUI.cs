using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI label;

    public void Show(string text)
    {
        if (label != null) label.text = text;
        if (group != null) group.alpha = 1f;
    }

    public void Hide()
    {
        if (group != null) group.alpha = 0f;
    }
}