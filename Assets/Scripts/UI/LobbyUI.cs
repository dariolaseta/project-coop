using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    
    [SerializeField] private LobbyCreateUI lobbyCreateUI;

    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    
    //TODO: REPLACE INSTANTIATE WITH OBJECT POOL
    
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        createLobbyButton.onClick.AddListener(CreateLobby);
        quickJoinButton.onClick.AddListener(QuickJoin);
        joinCodeButton.onClick.AddListener(JoinWithCode);
        
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            MultiplayerManager.Instance.SetPlayerName(newText);
        });

        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        
        UpdateLobbyList(new List<Lobby>());
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void CreateLobby()
    {
        lobbyCreateUI.Show();
    }

    private void JoinWithCode()
    {
        GameLobby.Instance.JoinWithCode(joinCodeInputField.text);
    }

    private void QuickJoin()
    {
        GameLobby.Instance.QuickJoin();
    }

    private void BackToMainMenu()
    {
        GameLobby.Instance.LeaveLobby();
        
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
