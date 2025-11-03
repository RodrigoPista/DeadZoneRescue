using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string startScene = "SampleScene";

    // Llamado por el botón START
    public void StartGame()
    {
        if (string.IsNullOrEmpty(startScene))
        {
            Debug.LogError("Scene name to load is empty.");
            return;
        }
        // 1) Resetear progreso persistente de quests
        QuestStateStore.ClearAll();

        // 2) Resetear flags de ítems / misiones en memoria
        QuestFlags.ResetAll();

        // 3) Limpiar tracker UI (si está presente en el menú)
        var tracker = FindObjectOfType<QuestTrackerUI>(true);
        tracker?.ClearAll();
        SceneManager.LoadScene(startScene);
    }

    // Llamado por el botón OPTIONS (por ahora placeholder)
    public void OpenOptions()
    {
        Debug.Log("Options pending (placeholder).");
        // Aquí luego abrís un Panel de opciones, o cargás otra escena, etc.
    }

    // Llamado por el botón EXIT (funciona solo en build)
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