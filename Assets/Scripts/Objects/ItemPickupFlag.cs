using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickupFlag : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Agarrar lata de comida [E]";

    public string GetPrompt() => prompt;

    public void Interact(GameObject interactor)
    {
        // Seteamos flag de "ya tengo la lata"
        QuestFlags.HasCannedFood = true;

        // (Opcional) Persistencia:
        // PlayerPrefs.SetInt("CANNED_FOOD_PICKED", 1);
        // PlayerPrefs.Save();

        // TODO: SFX/VFX de pickup
        Destroy(gameObject);
    }

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}