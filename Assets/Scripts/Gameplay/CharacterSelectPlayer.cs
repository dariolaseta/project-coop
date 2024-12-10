using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    
    [SerializeField] private GameObject readyGameObject;
    
    [SerializeField] private PlayerVisual playerVisual;

    [SerializeField] private Button kickButton;
    
    [SerializeField] private TMP_Text playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener(KickPlayer);
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();
            
            playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void KickPlayer()
    {
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(playerIndex);
        
        GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
        
        MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        
        //TODO Send player to main menu with reason
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
    }
}
