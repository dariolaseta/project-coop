using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitToMainMenuButton;
    
    [SerializeField] private GameObject settingsMenu;
    
    [SerializeField] private PlayerLogic playerLogic;
    
    [SerializeField] private VivoxPlayer vivoxPlayer;
    
    private void Awake()
    {
        resumeButton.onClick.AddListener(Resume);
        optionsButton.onClick.AddListener(Options);
        quitToMainMenuButton.onClick.AddListener(QuitToMainMenu);
    }

    private void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        playerLogic.ReturnToPrevPlayerState();
        gameObject.SetActive(false);
    }

    private void Options()
    {
        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void QuitToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        vivoxPlayer.OnUserLoggedOut();
        SceneManager.LoadScene("MainMenu");
    }
}
