using UnityEngine;

public interface IInteractable
{
    string GetPrompt();
    void Interact(GameObject interactor);
}
