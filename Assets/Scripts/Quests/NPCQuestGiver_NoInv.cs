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
    [SerializeField] private string questId = "quest.roberto.lata";
    [SerializeField] private string requiredFlagName = "HasCannedFood";
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

    [Header("Escena A Cargar")]
    [SerializeField] private bool loadSceneOnComplete = false;
    [SerializeField] private string nextSceneName = ""; // escribir el nombre exacto de la escena

    // -------- Lifecycle / Autowire --------
    void OnEnable()
    {
        TryAutoWireUI();
        // Restaurar estado persistido al entrar a la escena
        RestoreStateFromStore();

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LateWireNextFrame());
    }

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        TryAutoWireUI();
        // Tras carga de escena, aseguramos que el estado queda aplicado
        RestoreStateFromStore();
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

    // -------- Persistencia --------
    void RestoreStateFromStore()
    {
        var persisted = QuestStateStore.Get(questId);
        switch (persisted)
        {
            case QuestStage.Completed:
                state = State.Completed; // queda bloqueado en completada
                break;

            case QuestStage.NeedsItem:
                state = State.NeedsItem; // ya fue asignada previamente
                break;

            default:
                state = State.NotStarted;
                break;
        }
    }

    void SaveState(State s)
    {
        var stage = s == State.Completed ? QuestStage.Completed
                  : s == State.NeedsItem ? QuestStage.NeedsItem
                  : QuestStage.NotStarted;
        QuestStateStore.Set(questId, stage);
    }

    // -------- Interacción --------
    public void Interact(GameObject interactor)
    {
        var dlg = GetDialogue();
        var trk = GetTracker();

        switch (state)
        {
            case State.NotStarted:
                state = State.NeedsItem;
                SaveState(state); // ← persistir “waiting” para no volver a NotStarted
                dlg?.Show(startText);
                trk?.AddOrUpdateQuest(questId, questTitle, questDesc, 0, questTotal);
                break;

            case State.NeedsItem:
                if (CheckFlag(requiredFlagName))
                {
                    if (consumeFlagOnComplete) ClearFlag(requiredFlagName);
                    state = State.Completed;
                    SaveState(state); // ← persistir “completed”

                    dlg?.Show(completedText);
                    trk?.CompleteById(questId, "Completada");

                    if (loadSceneOnComplete)
                    {
                        var tracker = FindObjectOfType<QuestTrackerUI>(true);
                        if (tracker) tracker.gameObject.SetActive(false);

                        Time.timeScale = 1f;
                        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
                    }
                }
                else
                {
                    dlg?.Show(waitingText);
                }
                break;

            case State.Completed:
                // No retrocede nunca
                dlg?.Show(repeatedText);
                break;
        }
    }

    // -------- Flags (QuestFlags) --------
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

    // -------- Util --------
    void Reset() => gameObject.layer = LayerMask.NameToLayer("Interactable");
    public void ForceCompleted() { state = State.Completed; SaveState(state); }
}