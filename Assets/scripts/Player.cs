using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Player : MonoBehaviour {

    /*
        Problems:
            Gravity keeps affecting target, should reset velocity at apex of jump
            Player stops falling if he's moving left/right.  Actually, he falls slightly, unless he's pressed against a wall.
            Need logic to handle bumps in terrain.
    */

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    public float speed = 35f;
    public float jumpForce = 3500f;

    public Text debugText;
    public Text debugText2;

    private float xDir;
    private float yDir;

    private bool onGround = false;
    float groundRadius = 2.0f;
    public LayerMask whatIsGround;

    public Transform groundCheck;

    // Use this for initialization
    void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Clears direction of movement every update.
    /// </summary>
    void  ClearMovement()
    {
        xDir = 0.0f;
        yDir = 0.0f;
    }

    /// <summary>
    /// Gets the directions to move in as directed by input.
    /// </summary>
    void ReceiveInput()
    {
        xDir = Input.GetAxisRaw("Horizontal");
        yDir = Input.GetAxisRaw("Jump");
    }

    bool isGrounded()
    {
        return onGround;
    }

    void FixedUpdate ()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        

        var a = rb2D.velocity;
        debugText.text = "Velocity: " + a;
        debugText2.text = "On Ground: " + onGround + " Is Touching Stuff? " + rb2D.IsTouchingLayers(whatIsGround.value);

        ClearMovement();
        ReceiveInput();

        var jumpVelocity = 0f;

        if (isGrounded() && yDir > 0f && rb2D.velocity.y == 0) 
        {
            jumpVelocity = yDir * jumpForce;
        }

        rb2D.velocity = new Vector2(xDir * speed, rb2D.velocity.y);
        rb2D.AddForce(new Vector2(rb2D.velocity.x, jumpVelocity));
    }
}
