using System.Drawing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevelPhysics;
public abstract class moviminetoEnemigo : MonoBehaviour
{
   protected NavMeshAgent agent;

  [SerializeField] protected Transform player;
  [Header("")]
  [Tooltip("Capa donde los enemigos patrullan")]
  [SerializeField] protected LayerMask capaRutaEnemigo;
  [SerializeField] protected LayerMask capaJugador;
   Renderer colorEnemigo;
   
    //patrullar
    Vector3 walkPoint;
    bool walkPointSet;
   [Header("")]
   [Tooltip("Que tan lejos los enemigos caminan al patrullar")]
   [SerializeField] protected float walkPointRange;
    
    //ataque
   [SerializeField] protected float frecuenciaAtaque;
   protected bool yaAtaco;
   [SerializeField] protected float rangoVista, rangoAtaque;
    bool jugadorEnRangoVista, jugadorEnRangoAtaque;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colorEnemigo = GetComponent<Renderer>();
        
        
    }

    void Update()
    {
        jugadorEnRangoVista = Physics.CheckSphere(transform.position, rangoVista, capaJugador);
        jugadorEnRangoAtaque =  Physics.CheckSphere(transform.position, rangoAtaque, capaJugador);

        if (!jugadorEnRangoVista && !jugadorEnRangoAtaque) Patrullar();
        if (jugadorEnRangoVista && !jugadorEnRangoAtaque) Perseguir();
        if (jugadorEnRangoVista && jugadorEnRangoAtaque) Atacar();
        
    }


    protected void Patrullar()
    {
        agent.enabled = true;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        agent.stoppingDistance = 0f;

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            
        if (Physics.Raycast(walkPoint, -transform.up, 2f, capaRutaEnemigo))
        {
            walkPointSet = true;
        }
    }
   protected virtual void Perseguir()
    {
        agent.SetDestination(player.position);
        agent.stoppingDistance = 1.5f;
        }
  protected virtual void Atacar()
        {
            agent.stoppingDistance = 1.5f;
            transform.LookAt(player);
        if (!yaAtaco)
        {
            colorEnemigo.material.color = UnityEngine.Color.red;
            yaAtaco = true;
            Invoke(nameof(AttackReset), frecuenciaAtaque);
            
            }
        }
   protected void AttackReset()
    {
        yaAtaco = false;
        colorEnemigo.material.color = new UnityEngine.Color(0.1452474f, 0.6037736f, 0.1452474f, 1);
        }
    
    }

