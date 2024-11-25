using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DragonEnemy : MonoBehaviour
{
    // ---- Variables for enemy movement and health ----
    public GameObject projectilePrefab; // ---- The projectile prefab to shoot ----
    public float shootInterval = .5f; // ---- Time interval between shots ----
    public int maxHealth = 5000;
    private int currentHealth;

    // ---- Variables for player references and enemy state ----
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private PlayerGun playerGun;

    private SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    private Color originalColor;
    
    
    // ---- Floating Damage Numbers ----
    [SerializeField] private GameObject floatingDamageNumberPrefab;

    // ---- Event for enemy death ----
    public delegate void EnemyDeathHandler();
    public event EnemyDeathHandler OnEnemyDeath;

    // ---- Flag to indicate if the enemy is dead ----
    private bool isDead = false;

    // ---- Prefabs for particle effects and blobs----
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject blobPrefab;

    private Sprite originalSprite;
    public Sprite attackingSprite;
    
    public Slider healthSlider;
    
    public Transform mouthTransform;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            playerTransform = GameManager.Instance.player.transform;
            playerHealth = GameManager.Instance.player.GetComponent<PlayerHealth>();
            playerGun = GameManager.Instance.player.GetComponentInChildren<PlayerGun>(); // ---- Assuming the gun is a child of the player ----
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }

        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the enemy.");
        }
        else
        {
            originalColor = spriteRenderer.color;
            originalSprite = spriteRenderer.sprite; // ---- Initialize originalSprite ----
        }

        Debug.Log($"Enemy {gameObject.GetInstanceID()} script started.");

        // ---- Initialize the health slider ----
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        
        
        // ---- Start the shooting coroutine ----
        StartCoroutine(ShootAtPlayer());
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(shootInterval);
            int pattern = Random.Range(0, 6); // ---- Randomly select a shooting pattern ----
            switch (pattern)
            {
                case 0:
                    ShootCrossExplosivePattern();
                    break;
                case 1:
                    StartCoroutine(ShootSpiralPattern());
                    break;
                case 2:
                    ShootExplodingPattern();
                    break;
                case 3:
                    StartCoroutine(ShootZigzagPattern());
                    break;
                case 4:
                    StartCoroutine(ShootHelixPattern());
                    break;
                case 5:
                    StartCoroutine(ShootRandomScatterPattern());
                    break;
                
            }
            yield return new WaitForSeconds(0.1f);
            
        }
    }

    private IEnumerator ShootRandomScatterPattern()
    {
        int numProjectiles = 20;

        spriteRenderer.sprite = attackingSprite;
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = Random.Range(0f, 360f);
            float speed = Random.Range(2f, 10f);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = Instantiate(projectilePrefab, mouthTransform.position, rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;

            StartCoroutine(ChangeColorOverTime(projectile));
            Destroy(projectile, 5f);
            yield return new WaitForSeconds(0.05f);
        }
        spriteRenderer.sprite = originalSprite;
    }
    
    private void ShootCrossExplosivePattern()
    {
        GameObject projectile = Instantiate(projectilePrefab, mouthTransform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.left * 5f;

        StartCoroutine(CrossExplodeProjectile(projectile));
    }

    private IEnumerator CrossExplodeProjectile(GameObject projectile)
    {
        spriteRenderer.sprite = attackingSprite;
        yield return new WaitForSeconds(1f);
        Vector3 position = projectile.transform.position;
        Destroy(projectile);

        float[] angles = { 0f, 90f, 180f, 270f }; // Cross angles

        for (int i = 0; i < angles.Length; i++)
        {
            float angle = angles[i];
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject fragment = Instantiate(projectilePrefab, position, rotation);
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 5f;
            StartCoroutine(ChangeColorOverTime(fragment));
            StartCoroutine(SecondaryCrossExplodeProjectile(fragment));
            Destroy(fragment, 5f);
        }
        spriteRenderer.sprite = originalSprite;
    }

    private IEnumerator SecondaryCrossExplodeProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(1f);
        Vector3 position = projectile.transform.position;
        Destroy(projectile);

        float[] angles = { 0f, 90f, 180f, 270f }; // Cross angles

        for (int i = 0; i < angles.Length; i++)
        {
            float angle = angles[i];
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject fragment = Instantiate(projectilePrefab, position, rotation);
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 5f;
            StartCoroutine(ChangeColorOverTime(fragment));
            Destroy(fragment, 5f);
        }
    }
    
    
    private IEnumerator ShootSpiralPattern()
    {
        int numProjectiles = 12;
        float angleStep = 30f;
        float currentAngle = 0f;

        spriteRenderer.sprite = attackingSprite;
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = currentAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = Instantiate(projectilePrefab, mouthTransform.position, rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 5f;
            Destroy(projectile, 5f);
            currentAngle += 5f;
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.sprite = originalSprite;
    }

    
    private void ShootExplodingPattern()
    {
        GameObject projectile = Instantiate(projectilePrefab, mouthTransform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.left * 5f;

        StartCoroutine(ExplodeProjectile(projectile));
    }

    private IEnumerator ExplodeProjectile(GameObject projectile)
    {
        spriteRenderer.sprite = attackingSprite;
        yield return new WaitForSeconds(1f);
        Vector3 position = projectile.transform.position;
        Destroy(projectile);

        int numFragments = 8;
        float angleStep = 360f / numFragments;

        for (int i = 0; i < numFragments; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject fragment = Instantiate(projectilePrefab, position, rotation);
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 5f;
            StartCoroutine(ChangeColorOverTime(fragment));
            StartCoroutine(SecondaryExplodeProjectile(fragment));
            Destroy(fragment, 5f);
        }
        spriteRenderer.sprite = originalSprite;
    }
    
    private IEnumerator SecondaryExplodeProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(1f);
        Vector3 position = projectile.transform.position;
        Destroy(projectile);

        int numFragments = 4;
        float angleStep = 360f / numFragments;

        for (int i = 0; i < numFragments; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject fragment = Instantiate(projectilePrefab, position, rotation);
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 5f;
            StartCoroutine(ChangeColorOverTime(fragment));
            Destroy(fragment, 5f);
        }
    }
    
    private IEnumerator ShootHelixPattern()
    {
        int numProjectiles = 20;
        float angleStep = 18f;
        float radius = 1f;

        spriteRenderer.sprite = attackingSprite;
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            Vector3 position = mouthTransform.position + new Vector3(x, y, 0);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = Instantiate(projectilePrefab, position, rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(x, y).normalized * 5f;

            StartCoroutine(ChangeColorOverTime(projectile));
            Destroy(projectile, 5f);
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.sprite = originalSprite;
    }
    
    
    private IEnumerator ChangeColorOverTime(GameObject projectile)
    {
        SpriteRenderer sr = projectile.GetComponent<SpriteRenderer>();
        float duration = 1f;
        float elapsedTime = 0f;
        Color originalColor = sr.color;
        Color targetColor = Color.magenta;

        while (elapsedTime < duration)
        {
            sr.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = targetColor;
    }
    
    private IEnumerator ShootZigzagPattern()
    {
        int numProjectiles = 15;
        float zigzagAmplitude = 100f;
        float zigzagFrequency = 10f;

        spriteRenderer.sprite = attackingSprite;
        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = Mathf.Sin(i * zigzagFrequency) * zigzagAmplitude;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = Instantiate(projectilePrefab, mouthTransform.position, rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(-5f, Mathf.Sin(angle * Mathf.Deg2Rad) * 5f);
            
            
            StartCoroutine(ChangeColorOverTime(projectile));
            Destroy(projectile, 5f);
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.sprite = originalSprite;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log($"Enemy {gameObject.GetInstanceID()} hit by bullet.");
            if (playerGun != null)
            {
                var (damage, isCrit) = playerGun.GetDamage();
                TakeDamage(damage, isCrit);
                if (isCrit)
                {
                    GameManager.Instance.audioManager.PlaySFX("enemyCrit");
                }
                else
                {
                    GameManager.Instance.audioManager.PlaySFX("enemyHurt");
                }
            }

            Vector2 pointOfImpact = collision.ClosestPoint(transform.position);
            GameObject impactEffect = Instantiate(impactEffectPrefab, pointOfImpact, Quaternion.identity);
            StartCoroutine(DestroyImpactEffectAfterDelay(impactEffect, 2f));
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator DestroyImpactEffectAfterDelay(GameObject impactEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(impactEffect);
    }

    public void TakeDamage(int damage, bool isCrit)
    {
        Debug.Log($"TakeDamage called with damage: {damage}, isCrit: {isCrit}");

        currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.GetInstanceID()} took {damage} damage. IsCrit: {isCrit}. Current health: {currentHealth}");

        // ---- Update the health slider ----
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        ShowFloatingDamage(damage, transform.position, isCrit);

        StartCoroutine(FlashWhite());
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    
    public void ShowFloatingDamage(int damage, Vector3 position, bool isCrit)
    {
        FloatingDamageManager.Instance.ShowFloatingDamage(damage, position, isCrit);
    }
    
    
    
    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        
        // ---- Set the flag to indicate the enemy is dead ----
        if (isDead) return;
        isDead = true;
        GameManager.Instance.audioManager.PlaySFX("enemyDeath");
        Debug.Log($"Enemy {gameObject.GetInstanceID()} died.");
        if (OnEnemyDeath != null)
        {
            Debug.Log($"Invoking OnEnemyDeath event for enemy {gameObject.GetInstanceID()}.");
            OnEnemyDeath.Invoke();
        }

        // ---- Destroy any active floating damage numbers ----
        foreach (Transform child in transform)
        {
            if (child.CompareTag("FloatingDamageNumber"))
            {
                Destroy(child.gameObject);
            }
        }

        // ---- Instantiate a blob at the enemy's position ----
        Instantiate(blobPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}