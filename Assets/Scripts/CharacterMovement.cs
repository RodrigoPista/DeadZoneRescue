using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : HealthSystem
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float runAcceleration = 10f;
    public float doubleTapTime = 0.3f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float airControlMultiplier = 0f;

    [Header("Rotation")]
    public float rotationSpeed = 720f;

    [Header("Aiming")]
    public KeyCode aimKey = KeyCode.Mouse1; // right mouse button
    public float aimWalkSpeed = 2f;         // slower walk while aiming

    private CharacterController controller;

    private Vector3 inputDir;
    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;

    private bool isRunning = false;
    private bool isGrounded;

    private KeyCode lastKey;
    private float lastTapTime;

    private bool isAiming;

    public bool IsAiming => isAiming; // expose for camera

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Aiming toggle
        isAiming = Input.GetKey(aimKey);

        HandleInput();
        ApplyGravityAndJump();
        MoveCharacter();
        RotateCharacter();
    }

    void HandleInput()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveInput += camForward;
        if (Input.GetKey(KeyCode.S)) moveInput -= camForward;
        if (Input.GetKey(KeyCode.A)) moveInput -= camRight;
        if (Input.GetKey(KeyCode.D)) moveInput += camRight;

        moveInput = moveInput.normalized;

        if (isGrounded || airControlMultiplier > 0f)
        {
            inputDir = isGrounded ? moveInput : Vector3.Lerp(inputDir, moveInput, airControlMultiplier);
        }

        // Only detect double taps if not aiming
        if (!isAiming)
        {
            DetectDoubleTap(KeyCode.W);
            DetectDoubleTap(KeyCode.A);
            DetectDoubleTap(KeyCode.S);
            DetectDoubleTap(KeyCode.D);
        }
        else
        {
            isRunning = false; // cannot run while aiming
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void DetectDoubleTap(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastKey == key && Time.time - lastTapTime <= doubleTapTime)
            {
                isRunning = true;
            }
            else
            {
                // only cancel run if no key pressed at all
                isRunning = false;
            }

            lastKey = key;
            lastTapTime = Time.time;
        }

        if (Input.GetKeyUp(key))
        {
            // stop running only if all keys released
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
                isRunning = false;
        }
    }

    void ApplyGravityAndJump()
    {
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
    }

    void MoveCharacter()
    {
        if (inputDir != Vector3.zero)
        {
            float targetSpeed = walkSpeed;
            if (isRunning) targetSpeed = runSpeed;
            if (isAiming) targetSpeed = aimWalkSpeed;

            Vector3 targetVelocity = inputDir * targetSpeed;

            if (isRunning)
            {
                Vector3 minRunVelocity = inputDir * walkSpeed;
                if (currentVelocity.magnitude < minRunVelocity.magnitude)
                    currentVelocity = minRunVelocity;

                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, runAcceleration * Time.deltaTime);
            }
            else
            {
                currentVelocity = targetVelocity;
            }
        }
        else
        {
            if (isGrounded)
                currentVelocity = Vector3.zero;
        }

        Vector3 totalVelocity = currentVelocity + verticalVelocity;
        controller.Move(totalVelocity * Time.deltaTime);
    }

    void RotateCharacter()
    {
        if (isAiming)
        {
            // While aiming, rotate to face camera direction
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            if (camForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (inputDir != Vector3.zero && isGrounded)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    protected override void Die()
    {
        // Si la vida llega a 0
        if (IsDead)
        {
            SceneManager.LoadScene("Derrota"); // poné el nombre exacto de la escena
        }
    }
}

