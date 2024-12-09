using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(BackToMainMenu);
        readyButton.onClick.AddListener(CharacterSelectReady.Instance.SetPlayerReady);
    }

    private void BackToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu"); //TODO ENUM
    }
}
