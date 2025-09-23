using UnityEngine;

public class daño : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(50);
        }
    }
}
