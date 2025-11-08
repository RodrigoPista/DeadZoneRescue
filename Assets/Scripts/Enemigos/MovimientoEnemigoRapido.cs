using UnityEngine;
using UnityEngine.AI;

public class EnemigoRapido : moviminetoEnemigo
{
    [SerializeField] float velocidadAlAtacar = 10f;
    [SerializeField] float aceleracionAlAtacar = 10f;

    Renderer _rendererLocal;

    void Start()
    {
        // MUY IMPORTANTE: llamar a la inicialización de la clase base
        base.Invoke("Start", 0f); // alternativa si el Start base no es virtual
        // Si tu base tiene Start() normal, mejor exponelo como protected virtual y llamá base.Start();
        // protected override void Start() { base.Start(); ... } (si podés modificar la base)

        // Asegurar refs locales (por si el mesh está en un hijo)
        _rendererLocal = GetComponentInChildren<Renderer>();
        if (!_rendererLocal) _rendererLocal = GetComponent<Renderer>();

        agent = GetComponent<NavMeshAgent>();
        if (_rendererLocal) _rendererLocal.material.color = Color.yellow;
    }

    protected override void Atacar()
    {
        // si el player no existe o fue destruido, salimos y evitamos MissingReference
        if (!EnsurePlayer() || agent == null) return;

        if (_rendererLocal) _rendererLocal.material.color = Color.red;

        agent.autoBraking = false;
        agent.speed = velocidadAlAtacar;
        agent.acceleration = aceleracionAlAtacar;
        agent.angularSpeed = 720f; // 2 es demasiado bajo; 720º/s se siente bien

        agent.SetDestination(player.position);

        // Mirar solo en plano XZ (opcional)
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);
    }

    protected override void Perseguir()
    {
        if (!EnsurePlayer() || agent == null) return;

        if (_rendererLocal) _rendererLocal.material.color = Color.yellow;

        agent.autoBraking = true;
        agent.speed = 8f;
        agent.angularSpeed = 720f;

        agent.SetDestination(player.position);

        // Mirar solo en plano XZ (opcional)
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);
    }
}