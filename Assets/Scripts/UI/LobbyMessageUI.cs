using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame += MultiplayerManager_OnFailedToJoinGame;

        GameLobby.Instance.OnCreateLobbyStarted += MultiplayerManager_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += MultiplayerManager_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += MultiplayerManager_OnJoinStarted;
        GameLobby.Instance.OnJoinFailed += MultiplayerManager_OnJoinFailed;
        GameLobby.Instance.OnQuickJoinFailed += MultiplayerManager_OnQuickJoinFailed;
        
        
        Hide();
    }

    private void MultiplayerManager_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Nessuna stanza trovata");
    }

    private void MultiplayerManager_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Connessione alla stanza fallita");
    }

    private void MultiplayerManager_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Entrando nella stanza...");
    }

    private void MultiplayerManager_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Creazione della stanza fallita");
    }

    private void MultiplayerManager_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creazione della stanza in corso...");
    }

    private void MultiplayerManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Connessione fallita");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame -= MultiplayerManager_OnFailedToJoinGame;
    }
}
