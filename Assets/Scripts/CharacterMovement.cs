
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
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
    public KeyCode aimKey = KeyCode.Mouse1;
    public float aimWalkSpeed = 2f;

    [Header("Footstep Settings")]
    public AudioClip[] footstepSounds; // 🎵 varios sonidos de pasos
    public float baseStepInterval = 0.5f; // tiempo entre pasos
    private float stepTimer;
    private int lastStepIndex = -1;

    private CharacterController controller;
    private AudioSource audioSource;

    private Vector3 inputDir;
    private Vector3 currentVelocity;
    private Vector3 verticalVelocity;

    private bool isRunning = false;
    private bool isGrounded;
    private bool isAiming;

    private KeyCode lastKey;
    private float lastTapTime;

    public bool IsAiming => isAiming;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        // Configuración del AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // 3D sound si es un juego en 3D
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        isAiming = Input.GetKey(aimKey);

        HandleInput();
        ApplyGravityAndJump();
        MoveCharacter();
        RotateCharacter();
        HandleFootsteps(); // 👣 sonidos de pasos
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

        if (!isAiming)
        {
            DetectDoubleTap(KeyCode.W);
            DetectDoubleTap(KeyCode.A);
            DetectDoubleTap(KeyCode.S);
            DetectDoubleTap(KeyCode.D);
        }
        else
        {
            isRunning = false;
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
                isRunning = false;
            }

            lastKey = key;
            lastTapTime = Time.time;
        }

        if (Input.GetKeyUp(key))
        {
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
                !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
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

    void HandleFootsteps()
    {
        // 👣 Solo reproducir si está en el suelo y se mueve
        if (isGrounded && inputDir.magnitude > 0.1f)
        {
            float speedMultiplier = isRunning ? 0.7f : 1f;
            float interval = baseStepInterval * speedMultiplier;
            stepTimer += Time.deltaTime;

            if (stepTimer >= interval && footstepSounds.Length > 0)
            {
                // 🔀 elegir sonido al azar sin repetir el último
                int index;
                do
                {
                    index = Random.Range(0, footstepSounds.Length);
                } while (index == lastStepIndex && footstepSounds.Length > 1);

                lastStepIndex = index;

                AudioClip step = footstepSounds[index];
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(step);
                audioSource.pitch = 1f;

                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; // 🔇 si no se mueve o está saltando
        }
    }

    protected override void Die()
    {
        if (IsDead)
        {
            SceneManager.LoadScene("Derrota");
        }
    }
}
