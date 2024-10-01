using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    // ---- Gun/Bullet values ----
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float radius = 1f; // ---- Radius of the arc around the player ----

    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform.parent; // ---- Assuming the gun is a child of the player ----
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
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }
}
