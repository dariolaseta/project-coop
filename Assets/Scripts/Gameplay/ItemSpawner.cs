using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    public static ItemSpawner Instance { get; private set; }

    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
    }

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         SpawnItems();
    //     }
    // }

    public void SpawnItems()
    {
        if (!IsServer) return;

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount == 0)
            {
                GameObject itemToSpawn = items[UnityEngine.Random.Range(0, items.Count)];

                GameObject spawnedItem = Instantiate(itemToSpawn, spawnPoint.transform.position, Quaternion.identity);
                
                NetworkObject networkObject = spawnedItem.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                }
                else
                {
                    Debug.LogWarning("Spawned item does not have a NetworkObject component.");
                }
            }
        }
    }
}