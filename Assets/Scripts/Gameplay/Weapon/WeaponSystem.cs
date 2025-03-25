using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WeaponSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference fireAction;
    [SerializeField] private InputActionReference reloadAction;

    [SerializeField] private Transform shootingPointTransform;

    [SerializeField] private Weapon weapon;
    
    [SerializeField] private TMP_Text ammoCounter;
    
    private float fireRange = 0f;
    private float fireRate = 0f;
    private float nextTimeToFire = 0f;
    private float reloadTime = 0f;

    private int currentAmmo = 0;
    private int magazineSize = 0;
    private int totalAmmo = 30;
    private int maxAmmo = 0;
    
    private bool isReloading = false;
    
    private PlayerLogic playerLogic;

    private void Awake()
    {
        playerLogic = GetComponent<PlayerLogic>();
        
        fireAction.action.Enable();
        fireAction.action.performed += Shoot;
        reloadAction.action.Enable();
        reloadAction.action.performed += Reload;
        
        maxAmmo = weapon.MaxAmmo;
        fireRate = weapon.FireRate;
        currentAmmo = weapon.MaxAmmo;
        magazineSize = weapon.MagazineSize;
        fireRange = weapon.Range;
        reloadTime = weapon.ReloadTime;
        
        UpdateAmmoCounterLabel();
    }

    private void OnDestroy()
    {
        fireAction.action.Disable();
        fireAction.action.performed -= Shoot;
        reloadAction.action.Disable();
        reloadAction.action.performed -= Reload;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (!CanShoot()) return;
        
        Ray r = new Ray(shootingPointTransform.position, shootingPointTransform.forward);
        
        // Muzzleflash

        currentAmmo--;
        
        UpdateAmmoCounterLabel();
        // Shooting sound
        
        Debug.Log("SHOOT");
        
        nextTimeToFire = Time.time + 1f / fireRate;

        if (Physics.Raycast(r, out RaycastHit hit, fireRange))
        {
            HandleImpact(hit);
        }
    }

    private void HandleImpact(RaycastHit hit)
    {
        // Impact particle
        
        HealthSystem targetHealth = hit.collider.GetComponent<HealthSystem>();
        targetHealth?.TakeDamageServerRpc(weapon.Damage);
        
        Debug.Log($"Shoot {hit.transform.name} Current ammo: {currentAmmo}");
    }

    private void Reload(InputAction.CallbackContext obj)
    {
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        if (!CanReload()) yield break;
        
        Debug.Log("Reloading...");
        
        // Audio
        
        isReloading = true;
        
        yield return new WaitForSeconds(reloadTime);
        
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);
        
        currentAmmo += ammoToReload;
        totalAmmo -= ammoToReload;
        
        UpdateAmmoCounterLabel();
        
        isReloading = false;
    }
    
    // TODO: Aggiungere networkstates check
    private bool CanShoot()
    {
        return currentAmmo > 0 &&
               Time.time >= nextTimeToFire &&
               !isReloading &&
               GameManager.Instance.CurrentGameState.Value == GameManager.GameState.Playing &&
               playerLogic.PlayerState == PlayerState.Freeroam;
    }
    
    private bool CanReload() {
        
        return totalAmmo > 0 && 
               !isReloading && 
               currentAmmo < maxAmmo &&
               GameManager.Instance.CurrentGameState.Value == GameManager.GameState.Playing &&
               playerLogic.PlayerState == PlayerState.Freeroam;
    }
    
    private void UpdateAmmoCounterLabel() {
        
        ammoCounter.text = $"{currentAmmo} / {totalAmmo}";
    }
}
