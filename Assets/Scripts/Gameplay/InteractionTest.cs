using Unity.Netcode;
using UnityEngine;

public class InteractionTest : NetworkBehaviour, IInteractable
{
    [SerializeField] private ItemType itemType;
    private enum ItemType
    {
        Obtainable,
        Holding
    };

    [SerializeField] private float interactionRange = 2f;
    
    public void Interact()
    {
        InteractOverNetworkServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractOverNetworkServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        
        if (!IsPlayerInRange(clientId)) return;
        
        switch (itemType)
        {
            case ItemType.Obtainable:
                DeactivateObjectClientRpc();
                break;
            case ItemType.Holding:
                MakeObjectVisibleClientRpc(gameObject.name, clientId);
                break;
        }
    }

    private bool IsPlayerInRange(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client)) return false;
        
        Vector3 playerPosition = client.PlayerObject.transform.position;
        float distance = Vector3.Distance(playerPosition, transform.position);
        
        return distance <= interactionRange;
    }

    [ClientRpc]
    private void MakeObjectVisibleClientRpc(string objectName, ulong clientId)
    {
        GameObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;

        if (player)
        {
            string objectNameWithoutClone = objectName.Replace("(Clone)", "");

            Transform itemHolder = player.transform.Find("Items");
            
            if (!itemHolder) return;
            
            Transform targetObject = itemHolder.transform.Find(objectNameWithoutClone);

            if (targetObject)
            {
                targetObject.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    [ClientRpc]
    private void DeactivateObjectClientRpc()
    {
        gameObject.SetActive(false);
    }
}