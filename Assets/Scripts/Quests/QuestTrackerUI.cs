using UnityEngine;
using TMPro;
using System.Collections;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("FX")]
    [SerializeField] private float fadeTime = 0.15f;

    private Coroutine fadeCo;
    private int current = 0;
    private int total = 1;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        if (group) group.alpha = 0f;
    }

    public void ShowQuest(string title, string description, int start = 0, int goal = 1)
    {
        current = Mathf.Max(0, start);
        total = Mathf.Max(1, goal);

        if (titleText) titleText.text = title;
        if (descText) descText.text = description;
        if (progressText) progressText.text = $"{current}/{total}";

        FadeTo(1f);
    }

    public void SetProgress(int currentValue)
    {
        current = Mathf.Clamp(currentValue, 0, total);
        if (progressText) progressText.text = $"{current}/{total}";
    }

    public void Complete(string completedNote = "Completada")
    {
        // Marca como completa (podés estilizar: ✓, color verde, etc.)
        if (progressText) progressText.text = $"{total}/{total}";
        if (descText) descText.text = $"{descText.text}  ({completedNote})";

        // Ocultar suave luego de un tiempo (opcional)
        StopAllCoroutines();
        StartCoroutine(AutoHide(2.0f));
    }

    public void HideNow()
    {
        if (!group) return;
        if (fadeCo != null) StopCoroutine(fadeCo);
        group.alpha = 0f;
    }

    void FadeTo(float target)
    {
        if (!group) return;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeCo(target));
    }

    IEnumerator FadeCo(float target)
    {
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

    IEnumerator AutoHide(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        FadeTo(0f);
    }
}