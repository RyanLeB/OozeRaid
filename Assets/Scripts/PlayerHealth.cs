using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // ---- Health variables ----
    public int maxHealth = 100;
    public int baseMaxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;

    // ---- Results screen reference ----
    public ResultsScreen results;

    public float damageCooldown = 1f; // ---- Time in seconds between damage ticks ----
    private float lastDamageTime;

    public bool isDead = false; // ---- Flag to indicate if the player is dead ----

    // ---- Player currency reference ---- 
    private PlayerCurrency playerCurrency;

    // ---- Hit effect variables ----
    private SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float hitEffectDuration = 0.1f;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        lastDamageTime = -damageCooldown; // ---- Initialize to allow immediate damage ----
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        results = FindObjectOfType<ResultsScreen>();
        playerCurrency = GetComponent<PlayerCurrency>();

        // ---- Initialize hit effect variables ----
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
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
        else if (collision.gameObject.CompareTag("Blob"))
        {
            CollectBlob(collision.gameObject);
        }
    }

    // ---- Collect blob and add currency ----
    void CollectBlob(GameObject blob)
    {
        playerCurrency.AddRandomCurrency();
        BlobCollect blobScript = blob.GetComponent<BlobCollect>();
        if (blobScript != null)
        {
            blobScript.StartCoroutine(blobScript.AnimateBlob());
        }
        else
        {
            Destroy(blob); 
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        isDead = false;
    }


    public void DefaultHealth()
    {
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; 
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void DisablePlayerScripts()
    {
        MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) // ---- Ensure not to disable the PlayerHealth script itself ----
            {
                script.enabled = false;
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

        // ---- Trigger hit effect ----
        StartCoroutine(HitEffect());
    }

    void Die()
    {
        if (isDead) return; // ---- Exit if the player is already dead ----

        isDead = true; // ---- Set the flag to indicate the player is dead ----
        spriteRenderer.color = originalColor; // ---- Reset the sprite color ----
        
        // ---- Handle player death ----
        Debug.Log("Player Died");

        // ---- Show results screen ----
        ResultsScreen results = GameManager.Instance.resultsScreen;
        if (results != null)
        {
            Debug.Log("Showing results screen");
            int wave = FindObjectOfType<WaveManager>().currentWave; // ---- Get the current wave ----
            float time = Time.timeSinceLevelLoad; // ---- Get the elapsed time since the level started ----
            int currency = playerCurrency.currency; // ---- Amount of blobs collected ----
            
            results.ShowResults(wave, time, currency);
            Debug.Log("Results screen shown");
            
            
            DisablePlayerScripts(); // ---- Disable player scripts to prevent further input ----
             
        }
    }

    // ---- Coroutine to handle the hit effect ----
    IEnumerator HitEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitEffectDuration);
            spriteRenderer.color = originalColor;
        }
    }
    
    public void ResetSpriteColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // ---- Reset the sprite color to the original color ----
        }
    }
    
    
}