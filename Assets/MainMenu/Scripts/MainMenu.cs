using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string startScene = "SampleScene";

    [Header("Audio Settings")]
    [SerializeField] private AudioClip startButtonSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f; // 2D UI sound
        }
    }

    // Llamado por el botón START
    public void StartGame()
    {
        if (string.IsNullOrEmpty(startScene))
        {
            Debug.LogError("Scene name to load is empty.");
            return;
        }

        // 🔊 reproducir sonido antes de cargar la escena
        if (startButtonSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(startButtonSound);
        }

        // 🕐 esperar un poco antes de cargar la escena (para que se escuche el sonido completo)
        float delay = startButtonSound ? startButtonSound.length : 0f;
        Invoke(nameof(LoadStartScene), delay);
    }

    private void LoadStartScene()
    {
        SceneManager.LoadScene(startScene);
    }

    // Llamado por el botón OPTIONS
    public void OpenOptions()
    {
        Debug.Log("Options pending (placeholder).");
    }

    // Llamado por el botón EXIT
    public void QuitGame()
    {
        Debug.Log("Quit requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
