using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/WeaponData")]
public class Weapon : ScriptableObject
{
    [Header("General Informations")]
    [SerializeField] string weaponName;
    [SerializeField] string weaponDescription;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Sprite icon;
    
    [Header("Weapon Stats")]
    [SerializeField] int damage;
    [SerializeField] int magazineSize;
    [SerializeField] int maxAmmo;
    
    [SerializeField] float range;
    [SerializeField] float fireRate;
    [SerializeField] float reloadTime;
    
    [Header("Audio Clips")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;
    
    [Header("Particle Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem bulletImpactEffect;

    public string WeaponName => weaponName;
    public string WeaponDescription => weaponDescription;
    public Sprite Icon => icon;
    public GameObject WeaponPrefab => weaponPrefab;
    
    public int Damage => damage;
    public int MaxAmmo => maxAmmo;
    public int MagazineSize => magazineSize;
    
    public float Range => range;
    public float FireRate => fireRate;
    public float ReloadTime => reloadTime;
    
    public AudioClip ShootSound => shootSound;
    public AudioClip ReloadSound => reloadSound;
    
    public ParticleSystem MuzzleFlash => muzzleFlash;
    public ParticleSystem BulletImpactEffect => bulletImpactEffect;
}
