using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad; // Nombre exacto de la escena a cargar

    [Header("Audio Settings")]
    [SerializeField] private AudioClip doorSound; // Sonido al atravesar la puerta
    private AudioSource audioSource;

    private void Awake()
    {
        // Nos aseguramos de que haya un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuramos el AudioSource para sonidos 3D ambientales
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // 🔊 En 3D (si la puerta está en el mundo)
        audioSource.volume = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                // 🔊 reproducimos el sonido antes de cambiar de escena
                if (doorSound != null)
                {
                    audioSource.PlayOneShot(doorSound);

                    // 🕐 esperamos a que termine el sonido antes de cambiar de escena
                    float delay = doorSound.length;
                    Invoke(nameof(LoadScene), delay);
                }
                else
                {
                    // si no hay sonido, carga directo
                    LoadScene();
                }
            }
            else
            {
                Debug.LogError("SceneDoor: no se asignó ninguna escena para cargar.");
            }
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
