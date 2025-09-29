// Assets/Scripts/UI/HealthBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI healthText;

    HealthSystem target; // se resuelve en runtime

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
        TryFindPlayer(); // en la escena inicial ya lo deja linkeado
    }

    void OnEnable()
    {
        // por si cambiás de escena y perdés la ref
        if (!target) TryFindPlayer();
    }

    void Update()
    {
        if (!target)
        {
            // si todavía no lo encontró (p.ej. escena acaba de cargar), reintenta
            TryFindPlayer();
            return;
        }

        // actualizar barra y texto
        float pct = (float)target.Health / target.MaxHealth;
        if (slider) slider.value = pct;
        if (healthText) healthText.text = $"{target.Health} / {target.MaxHealth}";
    }

    void TryFindPlayer()
    {
        // Busca el objeto raíz con tag Player y su HealthSystem
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        target = player.GetComponent<HealthSystem>();
        // Si tu HealthSystem está en un hijo, podés usar GetComponentInChildren<HealthSystem>()
    }
}