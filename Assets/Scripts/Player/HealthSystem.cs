using System;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100; // TODO: Decidere se usare scriptable objects o dare un massimo uguale a tutti
    
    [SerializeField] private HealthSystemUI healthSystemUI;
    
    private NetworkVariable<int> currentHealth =  new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            healthSystemUI.UpdateHealthSlider(currentHealth.Value);
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamageServerRpc(10);
        }
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        healthSystemUI.UpdateHealthSlider(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        currentHealth.Value = Mathf.Max(0, currentHealth.Value - damage);
        
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        PlayerMovement playerMovement = GetComponentInParent<PlayerMovement>();
        playerMovement.enabled = false;
    }
}
