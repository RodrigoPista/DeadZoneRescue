using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit something with Health component
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        // Destroy bullet on impact
        Destroy(gameObject);
    }
}
