using UnityEngine;
using UnityEngine.AI;
public abstract class moviminetoEnemigo : MonoBehaviour
{
   protected NavMeshAgent agent;
  [SerializeField] protected Transform player;
  [SerializeField] protected LayerMask capaRutaEnemigo, capaJugador;
   protected Renderer colorEnemigo;
 

    //patrullar
    Vector3 walkPoint;
    bool walkPointSet;
   [SerializeField] protected float walkPointRange;
    [SerializeField]protected Material verdeNormal;
    //ataque
   [SerializeField] protected float frecuenciaAtaque;
   protected bool yaAtaco;
   [SerializeField] protected float rangoVista, rangoAtaque;
    bool jugadorEnRangoVista, jugadorEnRangoAtaque;
    [SerializeField] protected Material rojoAtaque;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colorEnemigo = GetComponent<Renderer>();
    }

    void Update()
    {
        jugadorEnRangoVista = Physics.CheckSphere(transform.position, rangoVista, capaJugador);
        jugadorEnRangoAtaque = Physics.CheckSphere(transform.position, rangoAtaque, capaJugador);

        if (!jugadorEnRangoVista && !jugadorEnRangoAtaque) Patrullar();
        if (jugadorEnRangoVista && !jugadorEnRangoAtaque) Perseguir();
        if (jugadorEnRangoVista && jugadorEnRangoAtaque) Atacar();
    }


    void Patrullar()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

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
    protected void Perseguir()
    {
        agent.SetDestination(player.position);
            
        }
       protected virtual void Atacar()
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);
        if (!yaAtaco)
        {
            colorEnemigo.material = rojoAtaque;
            yaAtaco = true;
            Invoke(nameof(AttackReset), frecuenciaAtaque);
            
            }
        }
   protected void AttackReset()
    {
        yaAtaco = false;
        colorEnemigo.material = verdeNormal;
        }
    
    }

