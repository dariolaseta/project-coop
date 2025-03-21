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
    [SerializeField] private TMP_Text playerPingText;

    private float pingUpdateTimer = 0f;
    private const float PING_UPDATE_INTERVAL = 1f;

    private void Awake()
    {
        kickButton.onClick.AddListener(KickPlayer);
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        UpdatePlayer();
    }

    private void Update()
    {
        pingUpdateTimer += Time.deltaTime;
        if (pingUpdateTimer >= PING_UPDATE_INTERVAL)
        {
            pingUpdateTimer = 0f;

            if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
            {
                PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromIndex(playerIndex);
                UpdatePingDisplay(playerData.clientId);
            }
        }
    }
    
    private void UpdatePingDisplay(ulong clientId)
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.NetworkConfig != null)
        {
            try
            {
                var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                if (transport != null)
                {
                    int ping = (int)transport.GetCurrentRtt(clientId);
                    playerPingText.text = $"{ping} ms";
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error getting ping: {e.Message}");
            }
        }
    
        playerPingText.text = "N/A";
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
            
            UpdatePingDisplay(playerData.clientId);
            
            bool isServer = NetworkManager.Singleton.IsServer;
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            bool isLocalPlayer = playerData.clientId == localClientId;
            kickButton.gameObject.SetActive(isServer && !isLocalPlayer);
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
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerManager_OnPlayerDataNetworkListChanged;
    }
}
