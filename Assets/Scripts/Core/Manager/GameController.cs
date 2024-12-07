using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState {FreeRoam, Pause, Task, Inventory}
public class GameController : MonoBehaviour
{
    //TODO Implement debug with states
    public static GameController Instance { get; private set; }
    
    private GameState state = GameState.FreeRoam;
    private GameState prevState;
    public GameState State => state;

    [SerializeField] private GameObject pauseMenu;
    
    [SerializeField] private InputActionReference pauseAction;
    
    //DEBUG
    private const string buildVer = "Pre alpha";
    [SerializeField] private TMP_Text buildVerString;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.started += PauseGame;
    }

    private void OnDisable()
    {
        pauseAction.action.Disable();
        pauseAction.action.started -= PauseGame;
    }

    private void Init()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        
        // DEBUG
        buildVerString.text = buildVer + " v. " + Application.version;
    }

    public void GoToPrevState()
    {
        state = prevState;
    }

    public void ChangeState(GameState newState)
    {
        prevState = state;
        state = newState;
    }

    public void PauseGame(InputAction.CallbackContext obj)
    {
        switch (state)
        {
            case GameState.FreeRoam:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ChangeState(GameState.Pause);
                pauseMenu.SetActive(true);
                break;
            case GameState.Pause:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GoToPrevState();
                pauseMenu.SetActive(false);
                break;
        }
    }
}
