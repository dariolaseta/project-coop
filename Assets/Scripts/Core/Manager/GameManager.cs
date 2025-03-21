using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private Transform playerPrefab;
    
    private NetworkVariable<int> playersReady = new NetworkVariable<int>();

    public enum GameState
    {
        Loading,
        Playing,
        GameOver
    }
    
    private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.Loading);
    public NetworkVariable<GameState> CurrentGameState => currentGameState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    public void PlayerReady()
    {
        playersReady.Value++;
        
        Debug.Log("ready players: " + playersReady.Value);
        
        Debug.Log($"Players Ready: {playersReady.Value} Players Connnected: {NetworkManager.Singleton.ConnectedClients.Count}");

        if (playersReady.Value == NetworkManager.Singleton.ConnectedClients.Count && IsServer)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        NotifyAllPlayersReadyClientRpc();
    }
    
    [ClientRpc]
    private void NotifyAllPlayersReadyClientRpc()
    {
        Debug.Log("GameManager: All players are ready, starting the game!");
        
        currentGameState.Value = GameState.Playing;

        UIManager.Instance.HideLoadingScreen();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}
