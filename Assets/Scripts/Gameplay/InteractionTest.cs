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
    
    public void Interact()
    {
        InteractOverNetworkServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractOverNetworkServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        
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