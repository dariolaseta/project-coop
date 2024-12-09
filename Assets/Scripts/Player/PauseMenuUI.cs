using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitToMainMenuButton;

    [SerializeField] private PlayerLogic playerLogic;
    
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
        
    }

    private void QuitToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
