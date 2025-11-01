using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCQuestGiver_NoInv : MonoBehaviour, IInteractable
{
    private enum State { NotStarted, NeedsItem, Completed }
    [SerializeField] private State state = State.NotStarted;

    [Header("UI (autocableable)")]
    [SerializeField] private DialogueUI dialogueUI;

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
    [SerializeField] private string talkPrompt = "Hablar [E]";

    public string GetPrompt() => talkPrompt;

    void OnEnable()
    {
        // Re-conectar al entrar a escena
        TryAutoWireUI();
        // Re-conectar cuando cualquier escena termina de cargar
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Por si la UI aparece 1 frame tarde (instanciada por script):
        StartCoroutine(LateWireNextFrame());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        TryAutoWireUI();
        // Y un late wire por si la UI se activa después del load
        StartCoroutine(LateWireNextFrame());
    }

    IEnumerator LateWireNextFrame()
    {
        yield return null;          // esperar 1 frame
        TryAutoWireUI();            // intentar nuevamente
    }

    void TryAutoWireUI()
    {
        //  Si ya está asignada y activa, listo
        if (dialogueUI && dialogueUI.isActiveAndEnabled) return;

        //  Búsqueda en la escena (incluye desactivados)
        if (!dialogueUI)
            dialogueUI = FindObjectOfType<DialogueUI>(true);
    }

    public void Interact(GameObject interactor)
    {
        switch (state)
        {
            case State.NotStarted:
                state = State.NeedsItem;
                ShowDialogue(startText);
                break;

            case State.NeedsItem:
                if (QuestFlags.HasCannedFood)
                {
                    QuestFlags.HasCannedFood = false;
                    QuestFlags.MissionCompleted = true;
                    state = State.Completed;
                    ShowDialogue(completedText);
                }
                else
                {
                    ShowDialogue(waitingText);
                }
                break;

            case State.Completed:
                ShowDialogue("Ya comí, ¡gracias de nuevo!");
                break;
        }
    }

    private void ShowDialogue(string text)
    {
        if (dialogueUI) dialogueUI.Show(text);
        else Debug.LogWarning($"[NPC] DialogueUI no encontrada en escena. Texto: {text}");
    }

    public void ForceCompleted() => state = State.Completed;
}