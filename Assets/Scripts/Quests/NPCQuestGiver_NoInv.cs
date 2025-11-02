using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;

public class NPCQuestGiver_NoInv : MonoBehaviour, IInteractable
{
    private enum State { NotStarted, NeedsItem, Completed }
    [SerializeField] private State state = State.NotStarted;

    [Header("UI (autocableo opcional)")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestTrackerUI questTracker;

    [Header("Identidad / Prompt")]
    [SerializeField] private string npcName = "Robert";
    public string GetPrompt() => $"<b><color=#FFD700>{npcName}</color></b>\nPresiona [E] para hablar";

    [Header("Misión (configurable por NPC)")]
    [SerializeField] private string questId = "quest.roberto.lata";           // ID único de misión
    [SerializeField] private string requiredFlagName = "HasCannedFood";       // nombre EXACTO del bool en QuestFlags
    [SerializeField] private bool consumeFlagOnComplete = true;

    [Header("Textos")]
    [TextArea]
    [SerializeField]
    private string startText =
        "Estoy muerto de hambre... ¿podés ir al mercado y traerme una lata?";
    [TextArea]
    [SerializeField]
    private string waitingText =
        "¿La conseguiste? Traeme una lata de comida, por favor.";
    [TextArea]
    [SerializeField]
    private string completedText =
        "¡Gracias! Me salvaste. Te debo una.";
    [TextArea]
    [SerializeField]
    private string repeatedText =
        "¡Gracias! Ya me ayudaste.";

    [Header("Tracker (texto visible)")]
    [SerializeField] private string questTitle = "Misión de Roberto";
    [SerializeField] private string questDesc = "Consigue la lata de comida en el mercado";
    [SerializeField] private int questTotal = 1;

    // ---------------- Lifecycle / Autowire ----------------
    void OnEnable()
    {
        TryAutoWireUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LateWireNextFrame());
    }

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        TryAutoWireUI();
        StartCoroutine(LateWireNextFrame());
    }

    IEnumerator LateWireNextFrame()
    {
        yield return null;
        TryAutoWireUI();
    }

    void TryAutoWireUI()
    {
        if (!dialogueUI) dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (!questTracker) questTracker = FindObjectOfType<QuestTrackerUI>(true);
    }

    DialogueUI GetDialogue()
    {
        if (dialogueUI && dialogueUI.isActiveAndEnabled) return dialogueUI;
        return FindObjectOfType<DialogueUI>(true);
    }

    QuestTrackerUI GetTracker()
    {
        if (questTracker && questTracker.isActiveAndEnabled) return questTracker;
        return FindObjectOfType<QuestTrackerUI>(true);
    }

    // ---------------- Interacción ----------------
    public void Interact(GameObject interactor)
    {
        var dlg = GetDialogue();
        var trk = GetTracker();

        switch (state)
        {
            case State.NotStarted:
                state = State.NeedsItem;
                dlg?.Show(startText);
                // Usar SIEMPRE API con ID para no pisar otras misiones:
                trk?.AddOrUpdateQuest(questId, questTitle, questDesc, 0, questTotal);
                break;

            case State.NeedsItem:
                if (CheckFlag(requiredFlagName))
                {
                    if (consumeFlagOnComplete) ClearFlag(requiredFlagName);
                    state = State.Completed;

                    dlg?.Show(completedText);
                    // Completar SOLO esta misión:
                    trk?.CompleteById(questId, "Completada");
                }
                else
                {
                    dlg?.Show(waitingText);
                }
                break;

            case State.Completed:
                dlg?.Show(repeatedText);
                break;
        }
    }

    // ---------------- Flags (via QuestFlags) ----------------
    static bool CheckFlag(string flagName)
    {
        var f = typeof(QuestFlags).GetField(flagName, BindingFlags.Public | BindingFlags.Static);
        return f != null && f.FieldType == typeof(bool) && (bool)f.GetValue(null);
    }

    static void ClearFlag(string flagName)
    {
        var f = typeof(QuestFlags).GetField(flagName, BindingFlags.Public | BindingFlags.Static);
        if (f != null && f.FieldType == typeof(bool)) f.SetValue(null, false);
    }

    // ---------------- Util ----------------
    void Reset() => gameObject.layer = LayerMask.NameToLayer("Interactable");
    public void ForceCompleted() => state = State.Completed;
}