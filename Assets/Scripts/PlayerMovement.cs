using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento del jugador.")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float groundDrag = 5f;

    [Header("Jump Settings")]
    [SerializeField] float jumbForce = 5f;          // (conservamos tu nombre para no perder valores del Inspector)
    [SerializeField] float jumpCooldown = 0.25f;
    bool canJump = true;

    [Header("Ground Detection")]
    [SerializeField] float playerHeight = 2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float radioSphereCheck = 0.1f;
    [SerializeField] Vector3 groundCheckOffset = new Vector3(0f, -0.9f, 0f); // aprox para cápsula altura ~2

    bool isGrounded;
    Rigidbody rb;
    Camera mainCamera;

    float horizontalInput;
    float verticalInput;
    Vector3 desiredVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Ground check cerca de los "pies"
        Vector3 checkPos = transform.position + groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPos, radioSphereCheck, groundLayer);
        Debug.DrawLine(checkPos, checkPos + Vector3.up * 0.05f, isGrounded ? Color.green : Color.red, 0f, false);

        HandleInput();
        HandleRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        // Movimiento
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            Jump();
        }
    }

    void HandleMovement()
    {
        // Dirección local (frente del jugador)
        Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        desiredVelocity = transform.TransformDirection(inputDir) * moveSpeed;

        // Aplicar velocidad en XZ, conservar Y
        Vector3 v = rb.linearVelocity;
        v.x = desiredVelocity.x;
        v.z = desiredVelocity.z;
        rb.linearVelocity = v;

        // Drag según si está en el piso
        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    void HandleRotation()
    {
        if (mainCamera == null) return;

        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 pointToLook = hit.point - transform.position;
            pointToLook.y = 0f;

            if (pointToLook.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(pointToLook);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    void Jump()
    {
        canJump = false;

        // reset vertical para saltos consistentes
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumbForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    void ResetJump() => canJump = true;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 checkPos = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPos, radioSphereCheck);
    }
}
