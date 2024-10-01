using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;

    public float damageCooldown = 1f; // ---- Time in seconds between damage ticks ----
    private float lastDamageTime;

    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = -damageCooldown; // Initialize to allow immediate damage
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                TakeDamage(10); // ---- Adjust damage value as needed ----
                lastDamageTime = Time.time;
            }
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        //  ---- Handle player death (e.g., restart level, show game over screen) ----
        Debug.Log("Player Died");
        // ---- Example: Destroy the player object ----
        Destroy(gameObject);
    }
}
