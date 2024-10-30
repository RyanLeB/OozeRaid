using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    // ---- Gun/Bullet values ----
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float radius = 0.5f; // ---- Radius of the arc around the player ----
    public int damage = 10;

    [SerializeField] private GameObject slimePiecePrefab;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private Sprite recoilSprite;
    private Sprite originalSprite;
    private Transform playerTransform;
    
    // ---- Variables for the gun's sprite ----
    private SpriteRenderer spriteRenderer;

    // ---- camera shake effect ----
    private CameraShake cameraShake;
    
    // ---- This stores the original fire point, so when the gun gets flipped, the fire point isn't adjusted ----
    private Vector3 originalFirePointPosition;
    
    

    void Start()
    {
        playerTransform = transform.parent; // ---- Assuming the gun is a child of the player ----
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        originalSprite = spriteRenderer.sprite;
        originalFirePointPosition = firePoint.localPosition; // Store the original local position
    }

    void Update()
    {
        Aim();
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
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

        // ---- Trigger camera shake ----
        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(0.1f, 0.5f));
        }

        // ---- Change the sprite to simulate recoil ----
        spriteRenderer.sprite = recoilSprite;
        Invoke("ResetSprite", 0.1f); // ---- Revert back to the original sprite after 0.1 seconds ----

        // ---- Instantiate and apply force to the slime piece ----
        GameObject slimePiece = Instantiate(slimePiecePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D slimeRb = slimePiece.GetComponent<Rigidbody2D>();

        // ---- Generate a random angle and calculate the direction ----
        float randomAngle = Random.Range(0f, 180f);
        Vector2 forceDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;

        slimeRb.AddForce(forceDirection * bulletSpeed * 0.5f, ForceMode2D.Impulse);

        // ---- Apply random rotation to the slime piece ----
        float randomTorque = Random.Range(-10f, 10f);
        slimeRb.AddTorque(randomTorque, ForceMode2D.Impulse);

        // ---- Start the coroutine to destroy the slime piece after a delay ----
        StartCoroutine(DestroySlimePieceAfterDelay(slimePiece, 5f));
    }

    
    // ---- Reset the sprite back to the original ----
    void ResetSprite()
    {
        spriteRenderer.sprite = originalSprite;
    }

    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }

    // ---- Getter method for the damage value ----
    public int GetDamage()
    {
        return damage;
    }
    
    
    // ---- Coroutine to destroy the slime piece after a delay ----
    private IEnumerator DestroySlimePieceAfterDelay(GameObject slimePiece, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(slimePiece);
    }
    
    
}
