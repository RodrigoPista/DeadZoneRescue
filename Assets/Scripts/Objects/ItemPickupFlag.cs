using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickupFlag : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectName = "Lata de comida";
    //[SerializeField] private string prompt = $"<b><color=#FFD700>{objectName}</color></b>\nPresiona [E] para agarrar";

    public string GetPrompt() => $"<b><color=#FFD700>{objectName}</color></b>\nPresiona [E] para agarrar";

    public void Interact(GameObject interactor)
    {
        QuestFlags.HasCannedFood = true;

        // Actualizar tracker a 1/1 si existe
        var tracker = FindObjectOfType<QuestTrackerUI>(true);
        tracker?.SetProgress(1);

        // TODO: SFX/VFX
        Destroy(gameObject);
    }

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}