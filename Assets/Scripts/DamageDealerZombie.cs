using UnityEngine;


[RequireComponent(typeof(Collider))]
public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int damage = 10;
    [SerializeField] float cooldown = 0.3f;
    [SerializeField] string targetTag = "Player";

    float lastHit;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Asegurar un Rigidbody en este GO para que el trigger funcione (kinematic)
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other) => TryHit(other);
    void OnTriggerStay(Collider other) => TryHit(other);

    void TryHit(Collider col)
    {
        // Resolver ROOT (si el collider que tocamos es un hijo)
        GameObject root = col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.transform.root.gameObject;

        // Filtrar por tag del root (Player)
        if (!root.CompareTag(targetTag))
            return;

        // Cooldown
        if (Time.time < lastHit + cooldown)
            return;

        // Buscar IDamageable en el root (tu HealthSystem lo implementa)
        if (root.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
            lastHit = Time.time;
#if UNITY_EDITOR
            Debug.Log($"{transform.root.name} golpeó a {root.name} por {damage} de daño");
#endif
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"{root.name} tiene tag {targetTag} pero no implementa IDamageable");
        }
#endif
    }
}