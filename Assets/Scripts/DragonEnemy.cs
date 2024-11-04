﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DragonEnemy : MonoBehaviour
{
    // ---- Variables for enemy movement and health ----
    public GameObject projectilePrefab; // ---- The projectile prefab to shoot ----
    public float shootInterval = 3f; // ---- Time interval between shots ----
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
    
    public Slider healthSlider;

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
            Shoot();
        }
    }

    private void Shoot()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float spreadAngle = 15f; // ---- Adjust the spread ----

            for (int i = -1; i <= 2; i++)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (i * spreadAngle);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                GameObject projectile = Instantiate(projectilePrefab, transform.position, rotation);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.velocity = rotation * Vector2.right * 5f; // ---- Adjust the speed ---- 
                Destroy(projectile, 5f); // ---- Destroy the projectile after 5 seconds ----
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log($"Enemy {gameObject.GetInstanceID()} hit by bullet.");
            if (playerGun != null)
            {
                TakeDamage(playerGun.GetDamage());
            }

            // ---- Get the point of impact ----
            Vector2 pointOfImpact = collision.ClosestPoint(transform.position);

            // ---- Instantiate and play the particle effect at the point of impact ----
            GameObject impactEffect = Instantiate(impactEffectPrefab, pointOfImpact, Quaternion.identity);

            // ---- Start the coroutine to destroy the particle effect after a delay ----
            StartCoroutine(DestroyImpactEffectAfterDelay(impactEffect, 2f)); // Adjust the delay as needed

            Destroy(collision.gameObject);
        }
    }

    private IEnumerator DestroyImpactEffectAfterDelay(GameObject impactEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(impactEffect);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.GetInstanceID()} took {damage} damage. Current health: {currentHealth}");
        StartCoroutine(FlashWhite());
        ShowFloatingDamage(damage, transform.position);

        // ---- Update the health slider ----
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    
    public void ShowFloatingDamage(int damage, Vector3 position)
    {
        if (floatingDamageNumberPrefab == null)
        {
            Debug.LogError("FloatingDamageNumberPrefab is not assigned.");
            return;
        }

        // ---- Instantiate the floating damage number prefab directly above the enemy ----
        GameObject floatingDamageNumber = Instantiate(floatingDamageNumberPrefab, position + Vector3.up * 1.5f, Quaternion.identity, transform);

        // ---- Adjust the scale of the prefab ----
        floatingDamageNumber.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // ---- Get the TextMeshPro component ----
        TextMeshPro textMesh = floatingDamageNumber.GetComponentInChildren<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component not found on FloatingDamageNumberPrefab.");
            return;
        }

        // ---- Set the damage text ----
        textMesh.text = damage.ToString();

        // ---- Start the bounce and fade out coroutine ----
        StartCoroutine(BounceAndFadeOut(floatingDamageNumber));
    }

    private IEnumerator BounceAndFadeOut(GameObject floatingDamageNumber)
    {
        TextMeshPro textMesh = floatingDamageNumber.GetComponent<TextMeshPro>();
        Color originalColor = textMesh.color;
        float duration = .5f; // ---- Duration of the fade out ----
        float bounceHeight = 0.8f; //  ---- Height of the bounce ----
        float elapsedTime = 0f;

        Vector3 originalPosition = floatingDamageNumber.transform.position;

        while (elapsedTime < duration)
        {
            // ---- Calculate the bounce effect ----
            float bounce = Mathf.Sin(Mathf.PI * elapsedTime / duration) * bounceHeight;
            floatingDamageNumber.transform.position = originalPosition + Vector3.up * bounce;

            // ---- Fade out effect ----
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(floatingDamageNumber);
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