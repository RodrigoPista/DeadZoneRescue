using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public float speed = 20f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Empuja la bala hacia adelante en el momento de instanciarla
        rb.linearVelocity = transform.forward * speed;

        // Destruir despu�s de un tiempo para no dejar basura
        Destroy(gameObject, lifeTime);
    }
}