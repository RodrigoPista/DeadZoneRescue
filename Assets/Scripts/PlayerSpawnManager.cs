using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] string spawnTag = "SpawnPoint";

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // esperar un frame para asegurar que todo esté activo en la nueva escena
        StartCoroutine(PlaceNextFrame());
    }

    IEnumerator PlaceNextFrame()
    {
        yield return null; // 1 frame
        var t = FindSpawnTransform();
        if (t != null)
        {
            transform.SetPositionAndRotation(t.position, t.rotation);
        }
        else
        {
            Debug.LogWarning("[Spawn] No se encontró SpawnPoint en la escena.");
        }
    }

    Transform FindSpawnTransform()
    {
        // 1) Activo por tag (rápido)
        var byTag = GameObject.FindWithTag(spawnTag);
        if (byTag) return byTag.transform;

        // 2) Con componente, incluso inactivos
        var byComp = Object.FindObjectsOfType<SpawnPoint>(true);
        if (byComp != null && byComp.Length > 0) return byComp[0].transform;

        // 3) Último recurso: recorrer transforms inactivos con ese tag
        foreach (var tr in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!tr.gameObject.scene.IsValid()) continue; // descarta assets/prefabs
            if (tr.CompareTag(spawnTag)) return tr;
        }
        return null;
    }
}