using System;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
    }

    public void DestroyItem(string itemName)
    {
        NetworkObjectReference itemRef = FindNetworkObjectByName(itemName);
        
        DestroyItemServerRpc(itemRef);
    }

    private NetworkObjectReference FindNetworkObjectByName(string itemName)
    {
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            if (networkObject.Value != null && networkObject.Value.gameObject.name == itemName)
            {
                return new NetworkObjectReference(networkObject.Value);
            }
        }
        return default;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyItemServerRpc(NetworkObjectReference itemRef)
    {
        itemRef.TryGet(out NetworkObject itemObject);

        if (itemObject == null) return;
        
        
        // Call ClientRpc to hide the item on the client side
        ClearItemOnParentClientRpc(itemRef);
    }

    [ClientRpc]
    private void ClearItemOnParentClientRpc(NetworkObjectReference itemRef)
    {
        itemRef.TryGet(out NetworkObject itemObject);
        GameObject item = itemObject.gameObject;
        
        // Deactivate the GameObject on the client side
        item.SetActive(false);
    }
}