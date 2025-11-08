using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("HealthSystem - Health")]
    [SerializeField] protected int maxHealth = 10; //Decreta la vida maxima
    protected int health; // Declara la variable para la vida

    public int Health => health;       //  publica solo lectura
    public int MaxHealth => maxHealth; //  publica solo lectura

    protected virtual void Awake()// Setea la vida a la vida maxima
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int amount)// 
    {
        health -= Mathf.Abs(amount);
        health = Mathf.Max(0, health);
        if (health == 0) Die();
    }

    public bool IsDead => health <= 0;

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
