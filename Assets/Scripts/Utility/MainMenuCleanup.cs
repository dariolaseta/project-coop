using System;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanup : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (MultiplayerManager.Instance != null)
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        
        if (GameLobby.Instance != null)
        {
            Destroy(GameLobby.Instance.gameObject);
        }

        if (NetworkLogger.Instance != null)
        {
            Destroy(NetworkLogger.Instance.gameObject);
        }

        if (PlayersManager.Instance != null)
        {
            Destroy(PlayersManager.Instance.gameObject);
        }
    }
}
