using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Object startScene = null;

    private Scene startSceneOld;

    // Llamado por el bot�n START
    public void StartGame()
    {
        if (startScene == null)
        {
            Debug.LogError("Scene to load is not assigned.");
            return;
        }
        SceneManager.LoadScene(startScene.name);
    }

    // Llamado por el bot�n OPTIONS (por ahora placeholder)
    public void OpenOptions()
    {
        Debug.Log("Options pending (placeholder).");
        // Aqu� luego abr�s un Panel de opciones, o carg�s otra escena, etc.
    }

    // Llamado por el bot�n EXIT (funciona solo en build)
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