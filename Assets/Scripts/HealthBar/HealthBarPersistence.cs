using UnityEngine;

public class HealthBarPersistence : MonoBehaviour
{
    void Awake()
    {
        // Evitar duplicados si otra escena ya trae un Canvas igual
        var bars = GameObject.FindGameObjectsWithTag("HealthBarUI");
        if (bars.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}