using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    // [SerializeField] private string sceneToLoad; // Nombre exacto de la escena a cargar
    [SerializeField] private Object sceneToLoad = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad.name);
        }
    }
}
