using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento del jugador.")]
    [SerializedField] float moveSpeed = 5f;
    [SerializedField] float rotationSpeed = 100f;
    [SerializedField] float groundDrag = 5f;

    [Header("Jump Settings")]
    [SerializedField] float jumbForce = 5f;
    [SerializedField] float jumpCooldown = 0.25f;
    private bool canJump = true;

    [Header("Ground Detenction")]
    [SerializedField] float playerHeight = 2f;
    [SerializedField] LayerMask groundLayer;
    [SerializedField] float radioSphereCheck = 0.1f;


    private bool isGrounded;
    Vector3 pointToLook;

    private Rigidbody rb;
    Vector3 movement;
    Vector3 velocity;

    private Camera mainCamera;

    float horizontalInput;
    float verticalInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        mainCamera = Camera.main;

        if (rb == null)
        { 
            Debug.LogError("Rigidbody no encontrado");
            
        }
        if (mainCamera == null)
        {
            Debug.LogError("Camara principal no encontrada");

        }

        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, radioSphereCheck, groundLayer);
        Debug.Log("Is Grounded :" + isGrounded);

        HandleInput();
        HandleMovement();
        HandleRotation();
        
    }
}
void FixedUpdate()
{

}

void HandleInput()
{
    //Inputs del movimiento
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    //Inputs del salto
    if (Input.GetkeyDown(KeyCode.Space) && isGrounded)
    {
        Jump();
    }
}

void HandleMovement()
{
    // Calcula la direccion del movimiento en relacion a la orientaicion del jugador
    movement = new Vector3(horizontalInput, 0f, verticalInput);

    // Calcula la velocidad en cada eje
    velocity = transform.TransformDirection(movement) * moveSpeed;

    // Aplica la velocidad
    rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

}

void HandleRotation()
{
    if(mainCamera == null) return;

    // Crea un rayo desde la posicion del mouse en la pantalla
    Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);

    // Objeto que almacena informacion sobre la colision del rayo
    RaycastHit hit;

    //Lanza el rayo y comprueba si colisiona con algo en la capa del suelo.
    //Mathf.Infinity asegura que el rayo viaje a una distancia infinita
    if(Physics.Raycast(cameraRay, out hitm Mathf.Infinity, groundLayer))
    {
        //El punto de colision es el lugar exacto donde el rayo golpeo la superficie.
        Vector3 pointToLook = hit.point - transform.position;
        pointToLook.y = 0f;

        //Dibuja una linea de depuracion en el editor de Unity para visualizar el rayo.
        Debug.DrawLine(cameraRay.origin, pointToLook, Color.red);

        //targe.transform.position= pointToLook;

        //Gira el jugador para que mira hacia el punto de colision, manteniendo su altura actual.
        //transform,LookAt(new Vector3(pointToLook.x, transform.position.y, pontToLook.z));
        Quaternion targetRotation = Quaternion.LookRotation(pointToLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);    
    }
}

private void Jump()
{
    Debug.Log("Jumb Key");
    // Resetea la velocidad vertical para saltos consistentes
    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

    rb.AddForce(transform.up * jumbForce, ForceMode.Impulse);
    isGrounded = false;
}

private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;

    if(platerHeight > 0)
    {
        Vector3 groundCheckPosition = transform.position;
        Gizmos.DrawWireSphere(groundCheckPosition, radioSphereCheck);

    }
}