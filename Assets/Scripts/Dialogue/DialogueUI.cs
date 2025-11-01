using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI textLabel;
    [Header("Ajustes")]
    [SerializeField] private float fadeTime = 0.1f;

    public bool IsVisible { get; private set; }

    Coroutine fadeCo;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        if (!textLabel) textLabel = GetComponentInChildren<TextMeshProUGUI>(true);
        if (group) group.alpha = 0f;
        IsVisible = false;
    }

    public void Show(string message)
    {
        if (textLabel) textLabel.text = message;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeTo(1f));
        IsVisible = true;
    }

    public void Hide()
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeTo(0f));
        IsVisible = false;
    }

    IEnumerator FadeTo(float target)
    {
        if (!group) yield break;
        float start = group.alpha;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeTime);
            yield return null;
        }
        group.alpha = target;
    }
}