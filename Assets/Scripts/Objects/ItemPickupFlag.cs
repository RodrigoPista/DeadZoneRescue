using UnityEngine;
using System.Reflection;

[RequireComponent(typeof(Collider))]
public class ItemPickupFlag : MonoBehaviour, IInteractable
{
    [Header("Configuración del objeto")]
    [SerializeField] private string objectName = "Objeto";
    [SerializeField] private string objectFlag = "HasCannedFood"; // nombre EXACTO del bool en QuestFlags
    [SerializeField] private string questId = "quest.roberto.lata"; // <-- asignalo por prefab

    public string GetPrompt() =>
        $"<b><color=#FFD700>{objectName}</color></b>\nPresiona [E] para agarrar";

    public void Interact(GameObject interactor)
    {
        // 1) Setear el flag en QuestFlags (reflexión)
        SetQuestFlagTrue(objectFlag);

        // 2) Actualizar SOLO la misión correspondiente por ID
        var tracker = FindObjectOfType<QuestTrackerUI>(true);
        if (tracker != null && !string.IsNullOrEmpty(questId))
        {
            tracker.SetProgressById(questId, 1);
        }

        // TODO: SFX/VFX
        Destroy(gameObject);
    }

    private void SetQuestFlagTrue(string flagName)
    {
        var field = typeof(QuestFlags).GetField(flagName, BindingFlags.Public | BindingFlags.Static);
        if (field != null && field.FieldType == typeof(bool))
        {
            field.SetValue(null, true);
        }
        else
        {
            Debug.LogWarning($"[ItemPickupFlag] No se encontró QuestFlags.{flagName} (bool).");
        }
    }

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}