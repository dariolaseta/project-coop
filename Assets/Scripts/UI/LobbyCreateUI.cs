using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Awake()
    {
        createPublicButton.onClick.AddListener(CreatePublicLobby);
        createPrivateButton.onClick.AddListener(CreatePrivateLobby);
        closeButton.onClick.AddListener(CloseLobbyCreation);
    }

    private void Start()
    {
        Hide();
    }

    private void CreatePublicLobby()
    {
        GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
    }

    private void CreatePrivateLobby()
    {
        GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
    }

    private void CloseLobbyCreation()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);   
    }
}
