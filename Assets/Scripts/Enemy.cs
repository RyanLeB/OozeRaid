using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float lungeSpeed = 5f;
    public float lungeDistance = 2f;
    public float stopDuration = 1f;
    public int maxHealth = 100;
    private int currentHealth;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private PlayerGun playerGun;
    private bool isLunging = false;
    private SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    private Color originalColor;

    public delegate void EnemyDeathHandler();
    public event EnemyDeathHandler OnEnemyDeath;

    private bool isDead = false;

    [SerializeField] private GameObject impactEffectPrefab; // Add this line

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            playerGun = player.GetComponentInChildren<PlayerGun>(); // Assuming the gun is a child of the player
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
        }

        Debug.Log($"Enemy {gameObject.GetInstanceID()} script started.");
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
        if (playerTransform != null && playerHealth != null && !playerHealth.isDead)
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
        else
        {
            return;
        }
    }

    private IEnumerator LungeTowardsPlayer()
    {
        isLunging = true;

        yield return new WaitForSeconds(stopDuration);
        if (playerTransform != null && playerHealth != null && !playerHealth.isDead)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float lungeTime = lungeDistance / lungeSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < lungeTime)
            {
                if (playerTransform == null || playerHealth == null || playerHealth.isDead)
                {
                    break;
                }
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, lungeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isLunging = false;
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

            // Get the point of impact
            Vector2 pointOfImpact = collision.ClosestPoint(transform.position);

            // Instantiate and play the particle effect at the point of impact
            GameObject impactEffect = Instantiate(impactEffectPrefab, pointOfImpact, Quaternion.identity);

            // Start the coroutine to destroy the particle effect after a delay
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
        if (currentHealth <= 0 && !isDead)
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
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"Enemy {gameObject.GetInstanceID()} died.");
        if (OnEnemyDeath != null)
        {
            Debug.Log($"Invoking OnEnemyDeath event for enemy {gameObject.GetInstanceID()}.");
            OnEnemyDeath.Invoke();
        }
        Destroy(gameObject);
    }
}