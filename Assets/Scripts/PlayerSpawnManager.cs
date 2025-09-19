using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        GameObject spawn = GameObject.FindWithTag("SpawnPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
}