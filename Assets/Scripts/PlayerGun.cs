using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{
    // ---- Gun/Bullet values ----
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float radius = 0.5f; // ---- Radius of the arc around the player ----
    public int damage = 5;
    public int baseDamage = 5;
    public float bulletLifetime = 5f; // ---- Lifetime of the bullet in seconds ----
    public float fireRate = 0.2f; // ---- Time between shots ----
    private bool isFiring = false;
    private Coroutine firingCoroutine;
    private bool isTempHoldToFireEnabled = false;
    
    
    // ---- Crit Values ----
    public float critChance = 0.1f; // ---- Base Chance of a critical hit ----
    public float critMultiplier = 2f; // ---- Multiplier for critical hits ----
    public float baseCritChance = 0.1f; // ---- Base crit chance value ----
    
    // ---- Ability variables ----
    public float abilityFireRate = 0.05f; // ---- Fire rate during ability ----
    public float abilityDuration = 5f; // ---- Duration of the ability ----
    public float abilityCooldown = 30f; // ---- Cooldown time for the ability ----
    private bool isAbilityActive = false;
    private bool isAbilityOnCooldown = false;

    // ---- UI elements ----
    public Slider cooldownSlider;

    [SerializeField] private GameObject slimePiecePrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private Sprite recoilSprite;
    private Sprite originalSprite;
    private Transform playerTransform;
    public GameObject superShotHUD; 
    public Image superShotReadyImage; 

    // ---- Variables for the gun's sprite ----
    private SpriteRenderer spriteRenderer;

    // ---- camera shake effect ----
    private CameraShake cameraShake;

    // ---- This stores the original fire point, so when the gun gets flipped, the fire point isn't adjusted ----
    private Vector3 originalFirePointPosition;
    
    // ---- Reference to the player upgrades script ----
    private PlayerUpgrades playerUpgrades;

    void Start()
    {
        playerTransform = transform.parent; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        originalSprite = spriteRenderer.sprite;
        originalFirePointPosition = firePoint.localPosition; // ---- Store the original local position ----
        playerUpgrades = GetComponentInParent<PlayerUpgrades>();
        
        
        // ---- Initialize the cooldown slider ----
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = abilityCooldown;
            cooldownSlider.value = abilityCooldown;
        }
        
        if (playerUpgrades.isAbilityUnlocked)
        {
            superShotHUD.SetActive(true);
        }
    }

    void Update()
    {
        Aim();
        bool canHoldToFire = playerUpgrades.isHoldToClickUnlocked || isTempHoldToFireEnabled;

        if (canHoldToFire)
        {
            if (Input.GetButtonDown("Fire1") && !isFiring)
            {
                isFiring = true;
                firingCoroutine = StartCoroutine(FireContinuously());
            }
            if (Input.GetButtonUp("Fire1"))
            {
                isFiring = false;
                if (firingCoroutine != null)
                {
                    StopCoroutine(firingCoroutine);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        if (!isAbilityOnCooldown)
        {
            superShotReadyImage.fillAmount = 1;
        }
        else
        {
            superShotReadyImage.fillAmount = 0;
        }

        if (!playerUpgrades.isAbilityUnlocked)
        {
            superShotHUD.SetActive(false);
        }

        if (playerUpgrades.isAbilityUnlocked && Input.GetKeyDown(KeyCode.E) && !isAbilityOnCooldown)
        {
            StartCoroutine(ActivateAbility());
        }

        
        if (isAbilityOnCooldown && cooldownSlider != null)
        {
            cooldownSlider.value += Time.deltaTime;
        }
    }

    void Aim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ---- Clamp the angle within the 180-degree range ----
        angle = Mathf.Clamp(angle, -180f, 180f);

        // ---- Calculate the new position of the gun along the arc ----
        Vector3 newPosition = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            0
        );

        // ---- Set the gun's position relative to the player ----
        transform.localPosition = newPosition;

        // ---- Rotate the gun to face the mouse position ----
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // ---- Flip the sprite based on the angle ----
        bool flipY = angle > 90 || angle < -90;
        spriteRenderer.flipY = flipY;

        // ---- Adjust the firePoint's local position based on the flip state ----
        firePoint.localPosition = flipY ? new Vector3(originalFirePointPosition.x, -originalFirePointPosition.y, originalFirePointPosition.z) : originalFirePointPosition;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
        GameManager.Instance.audioManager.PlaySFX("oozeShoot");
        var (damage, isCrit) = GetDamage();
        //Debug.Log($"Bullet Damage: {damage}, IsCrit: {isCrit}");

        Destroy(bullet, bulletLifetime);

        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.1f, 0.3f));
        }

        spriteRenderer.sprite = recoilSprite;
        Invoke("ResetSprite", 0.1f);

        GameObject slimePiece = Instantiate(slimePiecePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D slimeRb = slimePiece.GetComponent<Rigidbody2D>();

        float randomAngle = Random.Range(0f, 180f);
        Vector2 forceDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;

        slimeRb.AddForce(forceDirection * bulletSpeed * 0.5f, ForceMode2D.Impulse);
        float randomTorque = Random.Range(-10f, 10f);
        slimeRb.AddTorque(randomTorque, ForceMode2D.Impulse);

        StartCoroutine(DestroySlimePieceAfterDelay(slimePiece, 5f));
    }

    // ---- Coroutine to fire continuously while the player is holding the fire button ----
    private IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            //Debug.Log($"Firing with rate: {(isAbilityActive ? abilityFireRate : fireRate)}");
            Shoot();
            yield return new WaitForSecondsRealtime(isAbilityActive ? abilityFireRate : fireRate);
        }
    }

    // ---- Increase Crit Rate ----
    public void IncreaseCritRate(float amount)
    {
        critChance += amount;
    }
    
    public void ResetCritRate()
    {
        critChance = baseCritChance; 
    }
    
    // ---- Coroutine to activate the ability ----
    private IEnumerator ActivateAbility()
    {
        isAbilityActive = true;
        isAbilityOnCooldown = true;
        isTempHoldToFireEnabled = true; 

        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0;
        }

        yield return new WaitForSeconds(abilityDuration);

        isAbilityActive = false;
        isTempHoldToFireEnabled = false; 
        isFiring = false; 
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }

        yield return new WaitForSeconds(abilityCooldown - abilityDuration);
        isAbilityOnCooldown = false;

        if (cooldownSlider != null)
        {
            cooldownSlider.value = abilityCooldown;
        }
    }

    // ---- Reset the sprite back to the original ----
    void ResetSprite()
    {
        spriteRenderer.sprite = originalSprite;
    }

    // ---- Method to reset the ability ----
    public void ResetAbility()
    {
        isAbilityActive = false;
        isAbilityOnCooldown = false;
        if (cooldownSlider != null)
        {
            cooldownSlider.value = abilityCooldown;
        }
    }
    
    public void StopShooting()
    {
        
        isFiring = false;
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }
    }
    
    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }
    
    
    public void ResetDamage()
    {
        damage = baseDamage; 
    }

    // ---- Getter method for the damage value ----
    public (int damage, bool isCrit) GetDamage()
    {
        float randomValue = Random.value;
        bool isCrit = randomValue < critChance;
        int finalDamage = isCrit ? Mathf.RoundToInt(damage * critMultiplier) : damage;
        //Debug.Log($"Random Value: {randomValue}, Crit Chance: {critChance}, IsCrit: {isCrit}");
        return (finalDamage, isCrit);
    }

    // ---- Coroutine to destroy the slime piece after a delay ----
    private IEnumerator DestroySlimePieceAfterDelay(GameObject slimePiece, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(slimePiece);
    }
}