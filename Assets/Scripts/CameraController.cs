using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    [Header("Orbit Settings")]
    public float distance = 6f;
    public float height = 2f;
    public float sensitivity = 3f;
    public Vector2 pitchMinMax = new Vector2(-40f, 80f);

    [Header("Shoulder Offset")]
    public float horizontalOffset = 1.5f;       // default magnitude
    public KeyCode switchShoulderKey = KeyCode.V;
    public float shoulderSwitchSpeed = 5f;

    [Header("Aiming Zoom")]
    public float aimDistance = 3f;
    public float zoomSpeed = 5f;

    public CharacterMovement playerMovement;

    float yaw;
    float pitch;

    float currentDistance;
    float currentHorizontalOffset; // smoothed offset

    // track which side we’re on
    bool onLeftShoulder = false;

    void Start()
    {
        if(target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) target = player.transform;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        currentDistance = distance;
        currentHorizontalOffset = horizontalOffset;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // handle side switch input
        if (Input.GetKeyDown(switchShoulderKey))
        {
            onLeftShoulder = !onLeftShoulder;
        }

        // Orbit input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Smooth distance for aim zoom
        float desiredDistance = (playerMovement != null && playerMovement.IsAiming) ? aimDistance : distance;
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomSpeed);

        // Smooth horizontal offset flip
        float targetOffset = (onLeftShoulder ? -horizontalOffset : horizontalOffset);
        currentHorizontalOffset = Mathf.Lerp(currentHorizontalOffset, targetOffset, Time.deltaTime * shoulderSwitchSpeed);

        // Compute camera offset
        Vector3 baseOffset = new Vector3(0, height, -currentDistance);
        Vector3 shoulderOffset = rotation * new Vector3(currentHorizontalOffset, 0, 0);

        // Final camera transform
        transform.position = target.position + rotation * baseOffset + shoulderOffset;
        transform.rotation = rotation;
    }
}
