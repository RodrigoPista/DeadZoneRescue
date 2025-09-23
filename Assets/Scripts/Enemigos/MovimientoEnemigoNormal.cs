using System.Collections;
using System.ComponentModel.Design;
using UnityEngine.AI;
using UnityEngine;

public class EnemigoNormal : moviminetoEnemigo, IDamageable
{
    [SerializeField] int health = 100;
    public int Health
    {
        get { return health; }
        set { health = Mathf.Max(0, value); }
    }
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if(health == 0)
        {
            //que pasa cuando muere
        }
    }
}
