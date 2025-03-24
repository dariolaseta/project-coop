using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    public static PlayersManager Instance { get; private set; }

    public NetworkList<PlayerData> Players;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        Players = new NetworkList<PlayerData>();
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("Subscribing to events");
            
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
            
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                AddPlayer(client.Key);
            }
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            AddPlayer(clientId);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            RemovePlayer(clientId);
        }
    }

    private void AddPlayer(ulong clientId)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].clientId == clientId) return;
        }
        
        string playerName = "Player";

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            playerName = PlayerPrefs.GetString("PlayerName");
        }
        
        Players.Add(new PlayerData
        {
            clientId = clientId,
            playerName = playerName,
            colorId = (int)clientId % 4,
            //isMuted = false TODO: Aggiungere questo e settare bene colore e playerName
        });
    }

    private void RemovePlayer(ulong clientId)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].clientId == clientId)
            {
                Players.RemoveAt(i);
                break;
            }
        }
    }

    [ClientRpc]
    public void SyncPlayerNameClientRpc(ulong clientId, FixedString64Bytes newName)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].clientId == clientId)
            {
                var data = Players[i];
                data.playerName = newName;
                Players[i] = data;
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerDataServerRpc(PlayerData newData)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].clientId == newData.clientId)
            {
                Players[i] = newData;
                break;
            }
        }
    }
}