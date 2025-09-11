using UnityEngine;

public class movimientoCamara : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private const float YMin = -50.0f;
    private const float YMax = 50.0f;

    public Transform lookAt;
    public Transform Player;

   [SerializeField] float distancia = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    [SerializeField] float sensibilidad = 4.0f;

    // Update is called once per frame
    void LateUpdate()
    {
        currentX += Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        currentY += Input.GetAxis("Mouse Y") * -sensibilidad * Time.deltaTime;

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        Vector3 Direccion = new Vector3(0, 0, -distancia);
        Quaternion rotacion = Quaternion.Euler(currentY, currentX, 0);
        transform.position = lookAt.position + rotacion * Direccion;

        transform.LookAt(lookAt.position);
    }
}
