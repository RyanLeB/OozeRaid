using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;
    public ResultsScreen results;

    public float damageCooldown = 1f; // ---- Time in seconds between damage ticks ----
    private float lastDamageTime;

    public bool isDead = false; // ---- Flag to indicate if the player is dead ----

    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = -damageCooldown; // ---- Initialize to allow immediate damage ----
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return; // ---- Exit if the player is dead ----

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
        if (isDead) return; // ---- Exit if the player is dead ----

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
        if (isDead) return; // ---- Exit if the player is already dead ----

        isDead = true; // ---- Set the flag to indicate the player is dead ----

        // ---- Handle player death (e.g., restart level, show game over screen) ----
        Debug.Log("Player Died");

        // ---- Show results screen ----
        if (results != null)
        {
            int wave = 10; // 
            float time = 145.5f; 
            int currency = 1500; 
            results.ShowResults(wave, time, currency);
        }

        // ---- Example: Destroy the player object ----
        Destroy(gameObject);
    }
}
