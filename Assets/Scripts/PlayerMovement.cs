using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ---- Movement values ----
    public float speed;
    public float jump;
    
    float moveVelocity;

    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite movingSprite;
    public Sprite movingBackwardsSprite;

    public Rigidbody2D rb;
    bool isGrounded;

    

    void Update()
    {
        if (GetComponent<PlayerHealth>().isDead) return; // ---- Exit if the player is dead ----

        // ---- Grounded? ----
        if (isGrounded == true)
        {
            // ---- jumping ----
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);
            }
        }

        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput < 0)
        {
            moveVelocity = -speed; // ---- Move left ----
            spriteRenderer.sprite = movingSprite; // ---- Moving sprite ----
        }
        else if (horizontalInput > 0)
        {
            moveVelocity = speed; // ---- Move right ----
            spriteRenderer.sprite = movingSprite; // ---- Moving sprite ----
        }
        else
        {
            moveVelocity = 0; // ---- Stop moving ----
            spriteRenderer.sprite = idleSprite; // ---- Idle sprite ----
        }

        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);

        // ---- Flip sprite based on mouse position ----
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePosition - transform.position;

        // ---- Determine the appropriate sprite based on movement direction and mouse position ----
        if (moveVelocity != 0)
        {
            if ((moveVelocity < 0 && directionToMouse.x > 0) || (moveVelocity > 0 && directionToMouse.x < 0))
            {
                spriteRenderer.sprite = movingBackwardsSprite; // ---- Change to moving backward sprite ----
            }
            else
            {
                spriteRenderer.sprite = movingSprite; // ---- Change to moving sprite ----
            }
        }
        else
        {
            spriteRenderer.sprite = idleSprite; // ---- Change to idle sprite ----
        }

        // --- Set the flipX based on the mouse position ---- 
        spriteRenderer.flipX = directionToMouse.x < transform.position.x;
    }

    
    // ---- Check if the player is grounded ----
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Grounded is true");
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log("Grounded is false");
        isGrounded = false;
    }


    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }


}
