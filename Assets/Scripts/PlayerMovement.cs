using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ---- Movement values ----
    public float speed;
    public float jump;
    
    float moveVelocity;

    public Rigidbody2D rb;
    bool isGrounded;

    void Start()
    {
        
    }

    void Update()
    {
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
        }
        else if (horizontalInput > 0)
        {
            moveVelocity = speed; // ---- Move right ----
        }
        else
        {
            moveVelocity = 0; // ---- Stop moving ----
        }

        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
    }

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
}
