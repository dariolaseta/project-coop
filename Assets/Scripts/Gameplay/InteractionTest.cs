using Unity.Netcode;
using UnityEngine;

public class InteractionTest : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"Client {NetworkManager.LocalClientId} ha chiamato Interact");
        DeactivateObjectServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeactivateObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server ha ricevuto la richiesta da client {rpcParams.Receive.SenderClientId}");
        DeactivateObjectClientRpc();
    }

    [ClientRpc]
    private void DeactivateObjectClientRpc()
    {
        Debug.Log($"Disattivazione su client {NetworkManager.LocalClientId}");
        gameObject.SetActive(false);
    }
}