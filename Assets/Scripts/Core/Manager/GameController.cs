using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState {FreeRoam, Pause, Task, Inventory}
public class GameController : MonoBehaviour
{
    //TODO Refactor states to GameStates and PlayerStates
    public static GameController Instance { get; private set; }
    
    private GameState gameState = GameState.FreeRoam;
    private GameState prevGameState;
    public GameState GameState => gameState;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
    }

    public void GoToPrevGameState()
    {
        gameState = prevGameState;
    }

    public void ChangeGameState(GameState newState)
    {
        prevGameState = gameState;
        gameState = newState;
    }
}
