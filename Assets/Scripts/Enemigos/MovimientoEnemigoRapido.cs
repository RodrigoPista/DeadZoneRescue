using System.Collections;
using System.ComponentModel.Design;
using UnityEngine.AI;
using UnityEngine;

public class EnemigoRapido : moviminetoEnemigo, IDamageable
{   // Start is called once before the first execution of Update after the MonoBehaviour is created

   [SerializeField] float velocidadAlAtacar = 10;
   [SerializeField] float aceleracionAlAtacar = 10;
   [SerializeField] int health = 100;
   Renderer _renderer;



   void Start()
   {
      _renderer = GetComponent<Renderer>();
      agent = GetComponent<NavMeshAgent>();
      _renderer.material.color = Color.yellow;
   }
   protected override void Atacar()
   {
      _renderer.material.color = Color.red;
      agent.SetDestination(player.position);
      agent.autoBraking = false;
      agent.speed = velocidadAlAtacar;
      agent.acceleration = aceleracionAlAtacar;
      agent.angularSpeed = 2;


   }

   protected override void Perseguir()
   {
      _renderer.material.color = Color.yellow;
      agent.SetDestination(player.position);
      agent.speed = 8;
      agent.angularSpeed = 120;
      agent.autoBraking = true;
      transform.LookAt(player);

   }
    
    
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