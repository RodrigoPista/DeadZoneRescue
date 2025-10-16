using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    void OnEnable()
    {
        // Me suscribo al evento cuando este objeto está activo
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Limpieza: me desuscribo para evitar problemas
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se llama automáticamente cada vez que entra una escena nueva
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation; // opcional
        }
    }
}