using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    /*
        Problems:
            Gravity keeps affecting target, should reset vertex of fall at end of jump
            Player stops falling if he's moving left/right.  Actually, he falls slightly, unless he's pressed against a wall.
    */

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    public float speed = 1f;
    public float jumpSpeed = 1f;
    public float jumpLengthSec = 2f;
    public LayerMask blockingLayer;

    private float xDir;
    private float yDir;

    private bool isJumping = false;
    private IEnumerator<float> jumpProcedure;

    // Use this for initialization
    void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }


    //protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    //{
    //    Vector2 start = transform.position;
    //    Vector2 end = start + new Vector2(xDir, yDir);

    //    boxCollider.enabled = false;
    //    hit = Physics2D.Linecast(start, end, blockingLayer);
    //    boxCollider.enabled = true;

    //    if (hit.transform == null)
    //    {
    //        StartCoroutine(SmoothMovement(end));
    //        return true;
    //    }

    //    return false;
    //}

    protected IEnumerator<float> JumpTime(float jumpTime)
    {
        var originalJumpTime = jumpTime;

        var gravity = 5 * rb2D.gravityScale;

        while (jumpTime >= 0.0f)
        {
            var gravitySpeedKill = Mathf.Abs(jumpTime - originalJumpTime);

            yield return jumpSpeed * (1 - gravitySpeedKill);

            jumpTime -= Time.deltaTime;
        }
        isJumping = false;
        jumpProcedure = null;

        
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
        var jump = Input.GetAxisRaw("Jump");
        if (jump != 0.0f && !isJumping)
        {
            isJumping = true;
            jumpProcedure = JumpTime(jumpLengthSec);
        }

        xDir += Input.GetAxisRaw("Horizontal");
        if (isJumping)
        {
            yDir += 1.0f;
        }
    }

    void Update ()
    {

        ClearMovement();
        ReceiveInput();


        if (xDir != 0.0f || yDir != 0.0f)
        {
            Vector2 start = rb2D.position;
            Vector2 step = start + new Vector2(xDir, 0.0f);
            

            Vector3 positionAfterStep = Vector3.MoveTowards(rb2D.position, step, speed * Time.deltaTime);
            Vector3 positionAfterJump = positionAfterStep;

            // Continues a jump if we are jumping
            if (jumpProcedure != null && jumpProcedure.MoveNext())
            {
                Vector2 elevate = start + new Vector2(0.0f, 1.0f);
                var currJumpSpeed = jumpProcedure.Current;
                
                // The position of the object after jump.
                positionAfterJump = Vector3.MoveTowards(positionAfterStep, elevate, currJumpSpeed * Time.deltaTime);
            }

            // the final move.
            rb2D.MovePosition(positionAfterJump);
        }
    }
}
