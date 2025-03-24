using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    
    private Dictionary<string, VivoxParticipant> participants = new Dictionary<string, VivoxParticipant>();

    private void OnParticipantRemoved(VivoxParticipant participant)
    {
        if (participants.ContainsKey(participant.DisplayName))
        {
            participants.Remove(participant.DisplayName);
        }
    }

    private void OnParticipantAddedToChannel(VivoxParticipant participant)
    {
        Debug.Log("Channel name " + participant.ChannelName);
        Debug.Log("Display name " + participant.DisplayName);
        
        if (!participants.ContainsKey(participant.DisplayName))
        {
            participants.Add(participant.DisplayName, participant);
        }
        else
        {
            participants[participant.DisplayName] = participant;
        }
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            LoginToVivoxAsync();
            
            VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAddedToChannel;
            VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemoved;
            
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
        VivoxService.Instance.ParticipantAddedToChannel -= OnParticipantAddedToChannel;
        VivoxService.Instance.ParticipantRemovedFromChannel -= OnParticipantRemoved;
        
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
            
            string playerName = PlayerPrefs.GetString("PlayerName");
            
            LoginOptions loginOptions = new LoginOptions();
            loginOptions.DisplayName = playerName;
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
        NotifyPlayerReady();
    }

    private void NotifyPlayerReady()
    {
        NotifyPlayerReadyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void NotifyPlayerReadyServerRpc()
    {
        GameManager.Instance.PlayerReady();
    }

    public void OnUserLoggedOut()
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
    }

    public void SetPlayerVolume(VivoxParticipant participant, int volume)
    {
        participant.SetLocalVolume(volume);
    }

    public void SetMuteState(VivoxParticipant participant, bool isMuted)
    {
        if (isMuted)
            participant.MutePlayerLocally();
        else
            participant.UnmutePlayerLocally();
    }

    public VivoxParticipant GetParticipant(string displayName)
    {
        if (participants.TryGetValue(displayName, out VivoxParticipant participant))
        {
            return participant;
        }

        return null;
    }
}
