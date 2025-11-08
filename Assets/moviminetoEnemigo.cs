using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class moviminetoEnemigo : HealthSystem
{
    protected NavMeshAgent agent;

    [SerializeField] protected Transform player;

    [Header("Capas")]
    [SerializeField] protected LayerMask capaRutaEnemigo;
    [SerializeField] protected LayerMask capaJugador;

    Renderer colorEnemigo;

    // Patrulla
    Vector3 walkPoint;
    bool walkPointSet;
    [Header("Patrulla")]
    [SerializeField] protected float walkPointRange = 6f;

    // Ataque
    [Header("Combate")]
    [SerializeField] protected float frecuenciaAtaque = 0.8f;
    protected bool yaAtaco;
    [SerializeField] protected float rangoVista = 10f;
    [SerializeField] protected float rangoAtaque = 1.8f;

    bool jugadorEnRangoVista, jugadorEnRangoAtaque;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        CancelInvoke(); // por si quedó un Invoke de ataque pendiente
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // Al cambiar de escena, olvidá el player. Se re-resuelve en Update.
        player = null;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        colorEnemigo = GetComponentInChildren<Renderer>();
        EnsurePlayer(); // intenta resolver al inicio
    }

    void Update()
    {
        // Si no hay player (o fue destruido), intentá resolver; si no, salí.
        if (!EnsurePlayer())
        {
            if (agent) agent.ResetPath();
            return;
        }

        // Si el NavMeshAgent está inhabilitado o el objeto no está activo, salí.
        if (!agent || !isActiveAndEnabled) return;

        jugadorEnRangoVista = Physics.CheckSphere(transform.position, rangoVista, capaJugador, QueryTriggerInteraction.Ignore);
        jugadorEnRangoAtaque = Physics.CheckSphere(transform.position, rangoAtaque, capaJugador, QueryTriggerInteraction.Ignore);

        if (!jugadorEnRangoVista && !jugadorEnRangoAtaque) Patrullar();
        else if (jugadorEnRangoVista && !jugadorEnRangoAtaque) Perseguir();
        else if (jugadorEnRangoVista && jugadorEnRangoAtaque) Atacar();
    }

    // Intenta (re)resolver el player. Devuelve true si existe y está válido.
    protected bool EnsurePlayer()
    {
        // Si ya lo tenemos pero fue destruido, se verá como null (MissingReference)
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // Chequeo extra: que siga activo en escena
        return player != null && player.gameObject.scene.IsValid() && player.gameObject.activeInHierarchy;
    }

    protected void Patrullar()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.stoppingDistance = 0f;
            agent.SetDestination(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 1f)
                walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        var candidate = new Vector3(transform.position.x + randomX, transform.position.y + 1f, transform.position.z + randomZ);

        if (Physics.Raycast(candidate, Vector3.down, out var hit, 3f, capaRutaEnemigo, QueryTriggerInteraction.Ignore))
        {
            walkPoint = hit.point;
            walkPointSet = true;
        }
    }

    protected virtual void Perseguir()
    {
        if (!EnsurePlayer()) return;
        agent.stoppingDistance = 1.5f;
        agent.SetDestination(player.position);
    }

    protected virtual void Atacar()
    {
        if (!EnsurePlayer()) return;

        agent.stoppingDistance = 1.5f;

        // Mirar al jugador en plano XZ, con guardias por si se destruye entre frames
        Vector3 look = player.position - transform.position; // seguro porque EnsurePlayer()
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);

        if (!yaAtaco)
        {
            if (colorEnemigo) colorEnemigo.material.color = Color.red;
            yaAtaco = true;
            Invoke(nameof(AttackReset), frecuenciaAtaque);
            // acá iría la aplicación de daño por hitbox/overlap si corresponde
        }
    }

    protected void AttackReset()
    {
        yaAtaco = false;
        if (colorEnemigo) colorEnemigo.material.color = new Color(0.1452474f, 0.6037736f, 0.1452474f, 1f);
    }
}