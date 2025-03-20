using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject disconnectUI;
    
    [SerializeField] private Button returnToMainMenuButton;
    [SerializeField] private Button readyButton;

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
    }

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        
        disconnectUI.SetActive(false);
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

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
}
