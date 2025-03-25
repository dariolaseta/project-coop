using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private Transform playerPrefab;
    
    private NetworkVariable<int> playersReady = new NetworkVariable<int>();

    private Timer timer;

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
        
        timer = GetComponent<Timer>();
    }

    private void Start()
    {
        currentGameState.OnValueChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState previousValue, GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                UIManager.Instance.HideLoadingScreen();
                timer.StartTimer();
                break;
        }
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

        ChangeGameState(GameState.Playing);
    }

    private void ChangeGameState(GameState newState)
    {
        if (!IsServer) return;
        
        Debug.Log($"GameManager: Changing game state from {currentGameState.Value} to {newState}");
        
        GameState previousState = currentGameState.Value;
        currentGameState.Value = newState;
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

    public void GameOver()
    {
        GameOverClientRpc();
    }

    [ClientRpc]
    private void GameOverClientRpc()
    {
        ChangeGameState(GameState.GameOver);

        UIManager.Instance.ShowGameOverScreen();
    }
}
