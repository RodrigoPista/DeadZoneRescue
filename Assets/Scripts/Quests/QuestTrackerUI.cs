using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;      // se usa como listado multi-línea
    [SerializeField] private TextMeshProUGUI progressText;  // opcional (lo dejamos vacío)

    [Header("FX")]
    [SerializeField] private float fadeTime = 0.15f;

    [Header("Sorting")]
    [Tooltip("Incompletas primero; dentro de cada grupo, más recientes arriba.")]
    [SerializeField] private bool sortIncompleteFirst = true;
    [SerializeField] private bool sortNewestFirst = true;

    private Coroutine fadeCo;

    // ---- Modelo interno ----
    private class QuestEntry
    {
        public string QuestId;
        public string Title;
        public string Description;
        public int Current;
        public int Total;
        public bool Completed;       // Solo el NPC la pone en true (CompleteById)
        public int Version;          // contador incremental para orden de aparición
        public string CompletedNote; // ej: "Completada"
    }

    private readonly Dictionary<string, QuestEntry> _entries = new();
    private int _versionCounter = 0;

    // ---- Retrocompatibilidad (una sola “misión activa”) ----
    private string _lastQuestId = "_legacy_current";

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        if (group) group.alpha = 0f;
        if (titleText) titleText.text = $"<b><color=#FFD700>Misiones Activas</color></b>";
        if (progressText) progressText.text = "";
    }

    // ====== NUEVA API (multi-misión) ======

    public void AddOrUpdateQuest(string questId, string title, string description, int start = 0, int goal = 1)
    {
        if (string.IsNullOrEmpty(questId)) questId = MakeIdFromTitle(title);

        if (!_entries.TryGetValue(questId, out var e))
        {
            e = new QuestEntry
            {
                QuestId = questId,
                Title = title,
                Description = description,
                Current = Mathf.Max(0, start),
                Total = Mathf.Max(1, goal),
                Completed = false,       // al asignar, siempre arranca incompleta
                Version = ++_versionCounter,
                CompletedNote = null
            };
            _entries[questId] = e;
        }
        else
        {
            // Actualizamos texto y metas, pero NO cambiamos Completed automáticamente
            e.Title = title;
            e.Description = description;
            e.Total = Mathf.Max(1, goal);

            // Si ya estaba completada, no tocamos Current; si no, podés querer forzar el start
            if (!e.Completed)
            {
                e.Current = Mathf.Clamp(start, 0, e.Total);
                e.CompletedNote = null; // si vuelve a estado activo, limpiamos nota
            }

            e.Version = ++_versionCounter; // “reciente”
        }

        _lastQuestId = questId;
        RebuildListAndShow();
    }

    public void SetProgressById(string questId, int currentValue)
    {
        if (!_entries.TryGetValue(questId, out var e)) return;

        // Si ya está completada, ignoramos updates de progreso
        if (e.Completed)
        {
            RebuildListAndShow();
            return;
        }

        e.Current = Mathf.Clamp(currentValue, 0, e.Total);
        // IMPORTANTE: NO autocompletar acá (aunque sea 1/1). El NPC decide cuándo completar.
        // e.Completed permanece como estaba (false).
        e.Version = ++_versionCounter;

        // Por si venía una nota por error, la limpiamos mientras no esté completada:
        e.CompletedNote = null;

        RebuildListAndShow();
    }

    public void CompleteById(string questId, string completedNote = "Completada")
    {
        if (!_entries.TryGetValue(questId, out var e)) return;

        // Al completar desde el NPC, reflejamos 1/1 y estado final
        e.Current = e.Total;
        e.Completed = true;
        e.CompletedNote = completedNote;
        e.Version = ++_versionCounter;

        RebuildListAndShow();
    }

    public void RemoveById(string questId)
    {
        if (_entries.Remove(questId))
            RebuildListAndShow();
    }

    // ====== API ANTERIOR (retro-compatibilidad) ======
    public void ShowQuest(string title, string description, int start = 0, int goal = 1)
    {
        var id = MakeIdFromTitle(title);
        AddOrUpdateQuest(id, title, description, start, goal);
    }

    public void SetProgress(int currentValue)
    {
        if (string.IsNullOrEmpty(_lastQuestId)) return;
        SetProgressById(_lastQuestId, currentValue);
    }

    public void Complete(string completedNote = "Completada")
    {
        if (string.IsNullOrEmpty(_lastQuestId)) return;
        CompleteById(_lastQuestId, completedNote);
    }

    public void HideNow()
    {
        if (!group) return;
        if (fadeCo != null) StopCoroutine(fadeCo);
        group.alpha = 0f;
    }

    // ====== Render y animación ======

    void RebuildListAndShow()
    {
        if (descText)
        {
            descText.enableWordWrapping = true;

            // Orden
            IEnumerable<QuestEntry> ordered;

            if (sortIncompleteFirst)
            {
                var baseOrdered = _entries.Values.OrderBy(e => e.Completed ? 1 : 0);
                ordered = sortNewestFirst
                    ? baseOrdered.ThenByDescending(e => e.Version)
                    : baseOrdered.ThenBy(e => e.Version);
            }
            else
            {
                var baseOrdered = _entries.Values.OrderBy(e => 0); // sin prioridad
                ordered = sortNewestFirst
                    ? baseOrdered.ThenByDescending(e => e.Version)
                    : baseOrdered.ThenBy(e => e.Version);
            }

            // Construcción del texto multi-línea
            var sb = new StringBuilder();

            foreach (var e in ordered)
            {
                if (e.Completed)
                {
                    // Progreso + nota en verde (con ✓). Título y descripción tachados.
                    string note = string.IsNullOrEmpty(e.CompletedNote) ? "Completada" : e.CompletedNote;
                    string progress = $"<color=#5CFF5C>({e.Current}/{e.Total}<b>{note}</b>)</color>";

                    sb.AppendLine($"<s><b>• {e.Title}</b></s> {progress}");

                    if (!string.IsNullOrEmpty(e.Description))
                        sb.AppendLine($"    <s>{e.Description}</s>");
                }
                else
                {
                    // Incompleta: progreso gris, sin tachado
                    string progress = $"<color=#CCCCCC>({e.Current}/{e.Total})</color>";
                    sb.AppendLine($"<b>• {e.Title}</b> {progress}");

                    if (!string.IsNullOrEmpty(e.Description))
                        sb.AppendLine($"    {e.Description}");
                }

                sb.AppendLine();
            }

            descText.text = sb.ToString().TrimEnd();
        }

        if (progressText)
            progressText.text = "";

        FadeTo(1f);
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

    // Utility: generar ID si se usa la API vieja
    private string MakeIdFromTitle(string title)
    {
        if (string.IsNullOrEmpty(title)) return "_quest_" + (++_versionCounter);
        var id = new string(title.ToLowerInvariant()
                                   .Where(ch => char.IsLetterOrDigit(ch) || ch == ' ')
                                   .ToArray())
                                  .Replace(' ', '_');
        return $"quest.{id}";
    }

    public void ClearAll()
    {
        _entries.Clear();
        _versionCounter = 0;

        if (descText) descText.text = "";
        if (progressText) progressText.text = "";
        HideNow();
    }
}