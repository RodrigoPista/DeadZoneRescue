using UnityEngine;
using UnityEngine.AI;
public class moviminetoEnemigo : MonoBehaviour
{
   NavMeshAgent agent;
   public Transform player;
   public LayerMask capaPiso, capaJugador;
   public Renderer colorEnemigo;
 

    //patrullar
    Vector3 walkPoint;
    bool walkPointSet;
   [SerializeField] float walkPointRange;
    public Material verdeNormal;
    //ataque
   [SerializeField] float frecuenciaAtaque;
    bool yaAtaco;
   [SerializeField] float rangoVista, rangoAtaque;
    bool jugadorEnRangoVista, jugadorEnRangoAtaque;
    public Material rojoAtaque;

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
            
        if (Physics.Raycast(walkPoint, -transform.up, 2f, capaPiso))
        {
            walkPointSet = true;
        }
    }
       void Perseguir()
        {
            agent.SetDestination(player.position);
        }
        void Atacar()
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
    void AttackReset()
    {
        yaAtaco = false;
        colorEnemigo.material = verdeNormal;
        }
    
    }

