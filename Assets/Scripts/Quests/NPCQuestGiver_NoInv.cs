using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCQuestGiver_NoInv : MonoBehaviour, IInteractable
{
    private enum State { NotStarted, NeedsItem, Completed }
    [SerializeField] private State state = State.NotStarted;

    [Header("UI (opcionales; se autoconectan si están vacíos)")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private QuestTrackerUI questTracker;

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
    
    
    //[SerializeField] private string talkPrompt = "Hablar";
    [SerializeField] private string npcName = "Robert";

    [Header("Quest Tracker")]
    [SerializeField] private string questTitle = "Misión de Roberto";
    [SerializeField] private string questDesc = "Consigue la lata de comida en el mercado";
    [SerializeField] private int questTotal = 1;

    public string GetPrompt() =>
    $"<b><color=#FFD700>{npcName}</color></b>\nPresiona [E] para hablar";

    void OnEnable()
    {
        // Autocableo inicial y en carga de escena
        TryAutoWireUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LateWireNextFrame());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        TryAutoWireUI();
        StartCoroutine(LateWireNextFrame());
    }

    IEnumerator LateWireNextFrame()
    {
        yield return null; // 1 frame por si la UI aparece tarde
        TryAutoWireUI();
    }

    void TryAutoWireUI()
    {
        // 1) Si hay manager global, usarlo
        //if (UIRootManager.Instance)
        //{
           // if (!dialogueUI) dialogueUI = UIRootManager.Instance.dialogueUI;
           // if (!questTracker) questTracker = UIRootManager.Instance.questTracker;
       // }

        // 2) Buscar en escena como fallback
        if (!dialogueUI) dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (!questTracker) questTracker = FindObjectOfType<QuestTrackerUI>(true);
    }

    // Helper: obtener SIEMPRE la instancia viva del tracker
    QuestTrackerUI GetTracker()
    {
        // si la referencia serializada sigue viva, usarla
        if (questTracker && questTracker.isActiveAndEnabled) return questTracker;

        // probar manager global
        //if (UIRootManager.Instance && UIRootManager.Instance.questTracker)
            //return UIRootManager.Instance.questTracker;

        // buscar en escena
        return FindObjectOfType<QuestTrackerUI>(true);
    }

    // Helper idem para DialogueUI (opcional, por consistencia)
    DialogueUI GetDialogue()
    {
        if (dialogueUI && dialogueUI.isActiveAndEnabled) return dialogueUI;
        //if (UIRootManager.Instance && UIRootManager.Instance.dialogueUI)
            //return UIRootManager.Instance.dialogueUI;
        return FindObjectOfType<DialogueUI>(true);
    }

    public void Interact(GameObject interactor)
    {
        var dlg = GetDialogue();         // UI de diálogo viva
        var trk = GetTracker();          // UI de tracker viva

        switch (state)
        {
            case State.NotStarted:
                state = State.NeedsItem;
                dlg?.Show(startText);
                trk?.ShowQuest(questTitle, questDesc, 0, questTotal);  // 0/1
                break;

            case State.NeedsItem:
                if (QuestFlags.HasCannedFood)
                {
                    QuestFlags.HasCannedFood = false;
                    QuestFlags.MissionCompleted = true;
                    state = State.Completed;

                    dlg?.Show(completedText);
                    trk?.Complete("Completada");                        // 1/1 ✓ y oculta
                }
                else
                {
                    dlg?.Show(waitingText);
                }
                break;

            case State.Completed:
                dlg?.Show("Ya comí, ¡gracias de nuevo!");
                break;
        }
    }

    void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void ForceCompleted()
    {
        state = State.Completed;
    }
}