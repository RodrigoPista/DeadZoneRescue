using UnityEngine;

public class movimiento : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    LayerMask piso;
    [SerializeField] float velocidad = 1;
    Transform chequeoPiso;
    [SerializeField] float radioEsferaPiso;
    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 vectormovimiento = new Vector3(horizontal, 0f, vertical) * velocidad * Time.deltaTime;

        transform.Translate(vectormovimiento);

        
    }

    bool EnElPiso()
    {

         return Physics.CheckSphere(chequeoPiso.position, radioEsferaPiso, piso);
    }

      
}
