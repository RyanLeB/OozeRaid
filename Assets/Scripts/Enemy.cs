using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // ---- Speed at which the enemy moves ----
    public float lungeSpeed = 5f; // ---- Speed at which the enemy lunges ----
    public float lungeDistance = 2f; // ----- Distance at which the enemy lunges ----
    public float stopDuration = 1f; // ---- Duration for which the enemy stops before lunging ----
    public int maxHealth = 100; // ---- Maximum health of the enemy ----
    private int currentHealth;

    private Transform playerTransform;
    private bool isLunging = false;
    private SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f; // ---- Duration for which the enemy flashes white ----
    private Color originalColor;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the enemy.");
        }
        else
        {
            originalColor = spriteRenderer.color; // Store the original color
        }
    }

    private void Update()
    {
        if (!isLunging)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= lungeDistance)
            {
                StartCoroutine(LungeTowardsPlayer());
            }
            else
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            }
        }
    }

    private IEnumerator LungeTowardsPlayer()
    {
        isLunging = true;

        // ---- Stop for a second ----
        yield return new WaitForSeconds(stopDuration);

        // ---- Lunge towards the player ----
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float lungeTime = lungeDistance / lungeSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < lungeTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, lungeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isLunging = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(20); // Adjust damage value as needed
            Destroy(collision.gameObject); // Destroy the bullet on impact
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashWhite());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor; // Reset to the original color
        }
    }

    private void Die()
    {
        // Handle enemy death (e.g., play animation, drop loot)
        Destroy(gameObject);
    }
}
