using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    void Awake()
    {
        // Evitar duplicados si la escena ya trae otro Player
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}