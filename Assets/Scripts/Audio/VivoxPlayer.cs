using System;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.Serialization;


public class VivoxPlayer : NetworkBehaviour
{
    [SerializeField] private Transform localPlayerHead;
    private Vector3 lastPlayerHeadPos;
    
    [SerializeField] private AudioSettings audioSettings;
    
    private string voiceChannel = "Lobby";
    private bool isIn3DChannel = false;
    Channel3DProperties player3DProperties;

    private float nextPosUpdate;

    private int clientId;
    
    private void Start()
    {
        if (IsLocalPlayer)
        {
            LoginToVivoxAsync();
            
            VivoxService.Instance.LoggedIn += OnUserLoggedIn;
            VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        }
    }

    private void Update()
    {
        if (isIn3DChannel && IsLocalPlayer)
        {
            if (Time.time > nextPosUpdate)
            {
                UpdatePlayer3DPos();

                nextPosUpdate += 0.3f;
            }
        }
    }

    private void UpdatePlayer3DPos()
    {
        VivoxService.Instance.Set3DPosition(localPlayerHead.gameObject, voiceChannel);

        if (localPlayerHead.position != lastPlayerHeadPos)
        {
            lastPlayerHeadPos = localPlayerHead.position;
        }
    }

    private void OnDestroy()
    {
        if (IsLocalPlayer)
        {
            VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
            VivoxService.Instance.LoggedOut -= OnUserLoggedOut;
        }
    }

    private async void LoginToVivoxAsync()
    {
        if (IsLocalPlayer)
        {
            clientId = (int)NetworkManager.Singleton.LocalClientId;
            
            LoginOptions loginOptions = new LoginOptions();
            loginOptions.DisplayName = "Client" + clientId;
            loginOptions.EnableTTS = false;
            
            await VivoxService.Instance.LoginAsync(loginOptions);

            Join3DChannelAsync();
        }
    }

    private async void Join3DChannelAsync()
    {
        await VivoxService.Instance.JoinPositionalChannelAsync(voiceChannel, ChatCapability.AudioOnly, player3DProperties);
        isIn3DChannel = true;
        
        Debug.Log("Vivox: Successfully joined vivox 3d channel");
    }

    private void OnUserLoggedOut()
    {
        isIn3DChannel = false;

        VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Left all channels");
        
        VivoxService.Instance.LogoutAsync();
        Debug.Log("Logged out from Vivox");
    }

    private void OnUserLoggedIn()
    {
        if (!VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("Vivox: Cannot sign into Vivox, check your credentials and token settings");
            return;
        }
        
        audioSettings.PopulateInputDeviceList();
        audioSettings.PopulateOutputDeviceList();
        
        Debug.Log("Vivox: Client " + clientId + " logged in");
    }
}
