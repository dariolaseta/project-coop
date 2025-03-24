using System;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
    
    [Header("Player List")]
    [SerializeField] private GameObject playerEntityPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private Button muteButton;
    [SerializeField] private Slider volumeSlider;
    
    private void Awake()
    {
        resumeButton.onClick.AddListener(Resume);
        optionsButton.onClick.AddListener(Options);
        quitToMainMenuButton.onClick.AddListener(QuitToMainMenu);
    }

    private void OnEnable()
    {
        PlayersManager.Instance.Players.OnListChanged += UpdatePlayerList;
        UpdatePlayerList();
    }
    
    private void OnDisable()
    {
        if (PlayersManager.Instance != null)
        {
            PlayersManager.Instance.Players.OnListChanged -= UpdatePlayerList;
        }
    }
    
    // TODO: Object Pool in Alpha version
    private void UpdatePlayerList(NetworkListEvent<PlayerData> changeEvent = default)
    {
        ClearList();

        foreach (PlayerData player in PlayersManager.Instance.Players)
        {
            GameObject entity = Instantiate(playerEntityPrefab, playerListContent);
            entity.GetComponentInChildren<TMP_Text>().text = player.playerName.ToString();
            
            volumeSlider = entity.GetComponentInChildren<Slider>();
            volumeSlider.gameObject.SetActive(!IsLocalPlayer(player.clientId));
            volumeSlider.interactable = !player.isMuted;
            
            muteButton = entity.GetComponentInChildren<Button>();
            muteButton.onClick.AddListener(() => ToggleMutePlayer(player.clientId));
            muteButton.gameObject.SetActive(!IsLocalPlayer(player.clientId));
        }
    }

    private bool IsLocalPlayer(ulong clientId)
    {
        return clientId == NetworkManager.Singleton.LocalClientId;
    }

    private void ToggleMutePlayer(ulong playerClientId)
    {
        // TODO: Add muting & volume manager system
        Debug.Log($"Muting player with ID: {playerClientId}");
    }

    private void ClearList()
    {
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
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
