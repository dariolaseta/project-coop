using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject disconnectUI;
    [SerializeField] private GameObject warningUI;
    [SerializeField] private GameObject gameOverUI;
    
    [SerializeField] private Button returnToMainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button warningContinueButton;
    [SerializeField] private Button warningCancelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnToMainMenuGameOverButton;
    
    [SerializeField] private TMP_Text warningText;

    private bool isIntentionalDisconnect = false;
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        if (returnToMainMenuGameOverButton)
            returnToMainMenuGameOverButton.onClick.AddListener(ReturnToMainMenu);
        
        if (warningCancelButton)
            warningCancelButton.onClick.AddListener(CancelButtonClick);
        
        if (gameOverUI)
            gameOverUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        
        disconnectUI.SetActive(false);
        
        if (warningUI)
            warningUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && !isIntentionalDisconnect)
        {
            disconnectUI.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (readyButton)
                readyButton.gameObject.SetActive(false);
        }
    }

    private void ReturnToMainMenu()
    {
        isIntentionalDisconnect = true;
        
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        SceneManager.LoadScene("MainMenu");
    }

    private void CancelButtonClick()
    {
        CharacterSelectReady.Instance.UnreadyHost();
        warningUI.SetActive(false);
    }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

    public void ShowWarning(string message, UnityAction continueAction)
    {
        warningUI.SetActive(true);
        
        warningText.text = message;
        
        warningContinueButton.onClick.AddListener(continueAction);
    }

    public void ShowGameOverScreen()
    {
        ShowCursor();
        
        gameOverUI.SetActive(true);
        
        restartButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
