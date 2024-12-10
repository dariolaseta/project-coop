using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyCodeText;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        readyButton.onClick.AddListener(CharacterSelectReady.Instance.SetPlayerReady);
        
        Lobby lobby = GameLobby.Instance.GetLobby();
        
        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }

    private void BackToMainMenu()
    {
        GameLobby.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu"); //TODO ENUM
    }
}
