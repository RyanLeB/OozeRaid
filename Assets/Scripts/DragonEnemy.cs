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
                var (damage, isCrit) = playerGun.GetDamage();
                TakeDamage(damage, isCrit);
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