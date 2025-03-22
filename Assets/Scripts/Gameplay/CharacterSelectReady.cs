using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    
    private Dictionary<ulong, bool> playerReadyDictionary;
    
    //TODO ENUM
    [SerializeField] private string sceneToLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    public void ForceStartGame()
    {
        UIManager.Instance.ShowWarning("Forzare l'inizio della partita?", () =>
        {
            NetworkLogger.Instance.LogNetworkEvent("Forzato l'inizio della partita.");
            
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        });
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        
        bool allReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            ClientRpcParams hostRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { 0 }
                }
            };
            ShowStartPromptClientRpc(hostRpcParams);
        }
    }
    
    [ClientRpc]
    private void ShowStartPromptClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 1)
        {
            UIManager.Instance.ShowWarning("Sei solo nella stanza, continuare?", StartGame);
        }
        else
        {
            UIManager.Instance.ShowWarning("Tutti i giocatori sono pronti! Iniziare la partita?", StartGame);
        }
    }

    private void StartGame()
    {
        bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) 
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            NetworkLogger.Instance.LogNetworkEvent("Avviata la partita.");
            
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }
    
    public void UnreadyHost()
    {
        UnreadyHostServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnreadyHostServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderId = serverRpcParams.Receive.SenderClientId;
        
        // Solo l'host può rimuovere il proprio stato "pronto"
        if (senderId == 0 && playerReadyDictionary.ContainsKey(senderId))
        {
            playerReadyDictionary[senderId] = false;
            SetPlayerUnreadyClientRpc(senderId);
        }
    }
    
    [ClientRpc]
    private void SetPlayerUnreadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = false;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
