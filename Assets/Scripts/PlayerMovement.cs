using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ---- Movement values ----
    public float speed;
    public float jump;
    float moveVelocity;



    public Rigidbody2D rb;
    bool isGrounded;

    void Update()
    {
        // ---- Grounded? ----
        if (isGrounded == true)
        {
            // ---- jumping ----
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);

            }

        }

        moveVelocity = 0;

        // ---- Left Right Movement -----
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveVelocity = -speed;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveVelocity = speed;
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