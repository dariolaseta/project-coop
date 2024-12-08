using Unity.Netcode;
using UnityEngine;

public class InteractionTest : NetworkBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interact");
        DeactivateObjectClientRpc();
    }

    [ClientRpc]
    private void DeactivateObjectClientRpc()
    {
        if (IsOwner || IsServer)
        {
            gameObject.SetActive(false);
        }
    }
}
