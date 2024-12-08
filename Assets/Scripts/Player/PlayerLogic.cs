using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState  { Freeroam, Pause, Task, Spectating }
public class PlayerLogic : NetworkBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    
    [SerializeField] private PlayerState playerState = PlayerState.Freeroam;
    
    [SerializeField] private InputActionReference pauseAction;
    
    private PlayerState prevPlayerState;
    public PlayerState PlayerState => playerState;
    
    //DEBUG
    [SerializeField] private TMP_Text buildText;
    private const string BuildVer = "Pre Alpha v.";
    
    private void Awake()
    {
        buildText.text = BuildVer + " " + Application.version;
    }

    public void ChangePlayerState(PlayerState newState)
    {
        prevPlayerState = playerState;
        playerState = newState;
    }

    public void ReturnToPrevPlayerState()
    {
        playerState = prevPlayerState;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        pauseAction.action.Enable();
        pauseAction.action.started += PauseGame;
    }

    private void OnDisable()
    {
        if (!IsOwner) return;
        
        pauseAction.action.Disable();
        pauseAction.action.started -= PauseGame;
    }
    
    public void PauseGame(InputAction.CallbackContext obj)
    {
        switch (playerState)
        {
            case PlayerState.Freeroam:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ChangePlayerState(PlayerState.Pause);
                pauseMenu.SetActive(true);
                break;
            case PlayerState.Pause:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                ReturnToPrevPlayerState();
                pauseMenu.SetActive(false);
                break;
        }
    }
}
