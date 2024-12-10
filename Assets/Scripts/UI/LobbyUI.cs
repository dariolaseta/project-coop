using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        createLobbyButton.onClick.AddListener(CreateLobby);
        quickJoinButton.onClick.AddListener(QuickJoin);
    }

    private void CreateLobby()
    {
        GameLobby.Instance.CreateLobby("LobbyName", false);
    }

    private void QuickJoin()
    {
        GameLobby.Instance.QuickJoin();
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
