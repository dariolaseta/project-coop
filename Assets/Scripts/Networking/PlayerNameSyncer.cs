using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNameSyncer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            UpdatePlayerNameServerRpc(savedName);
        }
    }

    [ServerRpc]
    private void UpdatePlayerNameServerRpc(string newName)
    {
        for (int i = 0; i < PlayersManager.Instance.Players.Count; i++)
        {
            if (PlayersManager.Instance.Players[i].clientId == OwnerClientId)
            {
                var data = PlayersManager.Instance.Players[i];
                data.playerName = newName;
                PlayersManager.Instance.Players[i] = data;
                
                PlayersManager.Instance.SyncPlayerNameClientRpc(OwnerClientId, newName);
                break;
            }
        }
    }
}