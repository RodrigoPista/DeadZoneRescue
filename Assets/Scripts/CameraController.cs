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
    public float horizontalOffset = 1.5f;

    [Header("Aiming Zoom")]
    public float aimDistance = 3f;
    public float zoomSpeed = 5f;

    public CharacterMovement playerMovement;

    float yaw;
    float pitch;

    // keep our own smoothed distance
    float currentDistance;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        currentDistance = distance; // start at normal distance
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Orbit input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Smooth distance
        float desiredDistance = (playerMovement != null && playerMovement.IsAiming) ? aimDistance : distance;
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomSpeed);

        // Compute base offset in camera space
        Vector3 baseOffset = new Vector3(0, height, -currentDistance);
        Vector3 shoulderOffset = rotation * new Vector3(horizontalOffset, 0, 0);

        // Final position & rotation
        transform.position = target.position + rotation * baseOffset + shoulderOffset;
        transform.rotation = rotation;
    }
}
