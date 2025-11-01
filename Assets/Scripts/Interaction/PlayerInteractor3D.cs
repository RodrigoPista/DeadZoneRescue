using UnityEngine;
using System.Linq;

public class PlayerInteractor3D : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float radius = 3.0f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float rayDistance = 4f;
    [SerializeField] private bool useRaycastFirst = true;

    [Header("UI")]
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private DialogueUI dialogueUI; // <- opcional para ocultar diálogo al alejarse

    private IInteractable nearest;

    void Start()
    {
        if (!promptUI) promptUI = FindObjectOfType<InteractionPromptUI>(true);
        if (!dialogueUI) dialogueUI = FindObjectOfType<DialogueUI>(true);
    }

    void Update()
    {
        // Cancelar diálogo manualmente
        if (dialogueUI && dialogueUI.IsVisible && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)))
        {
            dialogueUI.Hide();
        }

        nearest = null;

        // 1) Raycast desde cámara
        if (useRaycastFirst)
        {
            var cam = Camera.main;
            if (cam)
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactableMask, QueryTriggerInteraction.Collide))
                {
                    nearest = GetI(hit.collider);
                }
            }
        }

        // 2) OverlapSphere fallback
        if (nearest == null)
        {
            var hits = Physics.OverlapSphere(transform.position, radius, interactableMask, QueryTriggerInteraction.Collide);
            if (hits.Length > 0)
            {
                var best = hits
                    .Select(h => new { H = h, I = GetI(h), D = Vector3.Distance(transform.position, h.transform.position) })
                    .Where(t => t.I != null)
                    .OrderBy(t => t.D)
                    .FirstOrDefault();

                if (best != null)
                    nearest = best.I;
            }
        }

        // 3) UI de prompt + ocultar diálogo si salgo de rango
        if (nearest != null)
        {
            promptUI?.Show(nearest.GetPrompt());

            if (Input.GetKeyDown(KeyCode.E))
                nearest.Interact(gameObject);
        }
        else
        {
            promptUI?.Hide();
            // si me alejé de todo interactuable, cierro diálogo si estuviera abierto
            if (dialogueUI && dialogueUI.IsVisible)
                dialogueUI.Hide();
        }
    }

    IInteractable GetI(Collider col)
    {
        if (!col) return null;
        return col.GetComponent<IInteractable>()
            ?? col.GetComponentInParent<IInteractable>()
            ?? col.GetComponentInChildren<IInteractable>();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}