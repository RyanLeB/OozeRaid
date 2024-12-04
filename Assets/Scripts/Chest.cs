using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    
    private int currentHealth;
    public int maxHealth = 100;
    public int numberOfBlobs = 5; 
    private WaveManager waveManager; 
    private PlayerGun playerGun;
    private bool isChestDestroyed = false;
    
    
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject blobPrefab;
    [SerializeField] private GameObject explosionPrefab;


    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager == null)
        {
            Debug.LogError("WaveManager not found in the scene.");
        }

        playerGun = GameManager.Instance.player.GetComponentInChildren<PlayerGun>();
        playerGun.isFiring = false;
        currentHealth = maxHealth;
        
        
        PlayExplosionEffect(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log($"Enemy {gameObject.GetInstanceID()} hit by bullet.");
            if (playerGun != null)
            {
                var (damage, isCrit) = playerGun.GetDamage();
                //Debug.Log($"Received Damage: {damage}, IsCrit: {isCrit}");
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
            GameManager.Instance.audioManager.PlaySFX("chestHitFloor");
            Destroy(collision.gameObject);
        }
        
    }
    
    
    public void TakeDamage(int damage, bool isCrit)
    {
        if (isChestDestroyed) return;

        currentHealth -= damage;
        //Debug.Log($"Enemy {gameObject.GetInstanceID()} took {damage} damage. IsCrit: {isCrit}. Current health: {currentHealth}");

        ShowFloatingDamage(damage, transform.position, isCrit);

        
        if (currentHealth <= 0)
        { 
            OpenChest();
            
        }
    }

    private IEnumerator DestroyImpactEffectAfterDelay(GameObject impactEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(impactEffect);
    }
    
    
    
    public void ShowFloatingDamage(int damage, Vector3 position, bool isCrit)
    {
        FloatingDamageManager.Instance.ShowFloatingDamage(damage, position, isCrit);
    }
    
    void OpenChest()
    {
        isChestDestroyed = true;
        
        if (waveManager.currentWave == 12)
        {
            numberOfBlobs *= 3; 
        }
        
        DropBlobs();
        GameManager.Instance.audioManager.PlaySFX("enemyDeath");
        PlayExplosionEffect(); 
        StartCoroutine(DestroyChestAfterDelay(2f));
        
        
    }

    void DropBlobs()
    {
        StartCoroutine(DropBlobsSequence());
    }

    
    private IEnumerator DestroyChestAfterDelay(float delay)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; 
        }
        if (circleCollider2D != null)
        {
            circleCollider2D.enabled = false; 
        }
        
        yield return new WaitForSeconds(delay);
        waveManager.OnChestOpened();
        Destroy(gameObject); 
    }
    
    private IEnumerator DropBlobsSequence()
    {
        for (int i = 0; i < numberOfBlobs; i++)
        {
            Instantiate(blobPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.audioManager.PlaySFX("blobDrop");
            yield return new WaitForSeconds(0.2f); 
        }
    }
    
    void PlayExplosionEffect()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
            Destroy(explosion, particleSystem.main.duration);
        }
    }
}