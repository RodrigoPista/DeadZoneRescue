using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraPersistant : MonoBehaviour
{
    void Awake()
    {
        // Evitar duplicados en la primera escena
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        if (cameras.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destruir cámaras de ESCENA que no sean esta
        foreach (var cam in Camera.allCameras)
        {
            if (cam.gameObject != gameObject)
            {
                // Si esa cámara NO es la persistente, volala
                Destroy(cam.gameObject);
            }
        }
    }
}