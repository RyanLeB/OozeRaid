using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    // ---- This script enhances the jump by making it feel less floaty, 
    // to do this it simply adds a fall multiplier and a low jump mechanic ----

    
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        
        // ---- If the player is falling (velocity.y < 0),
        // apply the fall multiplier to make the fall faster ----

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        
        // ---- If the player is jumping (velocity.y > 0) and the jump button is not held down,
        // apply the low jump multiplier to make the jump shorter ----
        
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

}
