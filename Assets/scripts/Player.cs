using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour {

    /*
        Problems:
            Gravity keeps affecting target, should reset velocity at apex of jump
            Player stops falling if he's moving left/right.  Actually, he falls slightly, unless he's pressed against a wall.
            Need logic to handle bumps in terrain.
    */

    enum AirState
    {
        Grounded,
        LeavingGround,
        FirstJump,
        FirstJumpNoKey,
        SecondJump,
        SecondJumpNoKey,
        FallNoJump
    }

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public LayerMask whatIsGround;

    // physics appliers
    public float maxSpeed = 35f;
    public float startSpeed = 15f;
    public float secondsToMaxSpeed = 1.5f;
    public float jumpForce = 1000f;
    public float stepForce = 500f;

    // Just some text
    public Text debugText;
    public Text debugText2;

    // inputs
    private float xDir;
    private float yDir;
    private bool activateDown;

    // States
    private AirState airState = AirState.FirstJump;
    private float currentRunForce = 0f;
    private float currentDirection = 1f;

    // Physical Properties
    float groundRadius = 2.0f;
    float stepRadius = .2f;

    public Transform groundCheck;
    public Transform lStepCheck;
    public Transform rStepCheck;
    public Transform lMaxStep;
    public Transform rMaxStep;

    public GameObject redLightPrefab;
    private GameObject redLight;

    // Use this for initialization
    void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        activateDown = Input.GetAxisRaw("Fire1") == 0 ? false : true;
    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
    }

    void FixedUpdate ()
    {
        ClearMovement();
        ReceiveInput();

        MovePlayer();

        UpdateSpriteState();

        if (activateDown)
        {
            if (redLight == null)
            {
                redLight = Instantiate(redLightPrefab);
                
                var a = redLight.GetComponent<SpriteRenderer>();
                a.color = new Color(1f, 1f, 1f, .5f);

            }
            redLight.transform.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        }
        else
        {
            if(redLight != null)
            {
                Destroy(redLight);
            }
        }

    }

    void Update()
    {

    }

    void MovePlayer()
    {
        var onRightStep = Physics2D.OverlapCircle(rStepCheck.position, stepRadius, whatIsGround);
        var onLeftStep = Physics2D.OverlapCircle(lStepCheck.position, stepRadius, whatIsGround);


        debugText.text = "Velocity: " + rb2D.velocity;
        debugText2.text = "Air State: " + airState + " Is Touching Stuff? " + rb2D.IsTouchingLayers(whatIsGround.value);

        determinePlayerRunForceAndDirection();

        rb2D.velocity = new Vector2(currentDirection * currentRunForce, rb2D.velocity.y);

        var jumpVelocity = getJumpVelocity();



        rb2D.AddForce(new Vector2(rb2D.velocity.x, jumpVelocity));

    }

    private void determinePlayerRunForceAndDirection()
    {
        bool hasTurned = false;

        if (xDir != 0)
        {
            if (xDir != currentDirection)
            {
                hasTurned = true; 
            }
            currentDirection = xDir;
        }

        var speedDiff = maxSpeed - startSpeed;
        var speedUpRatio = Time.deltaTime / secondsToMaxSpeed;
        var speedDiffForTimeDiff = speedUpRatio * speedDiff;
        
        // player is just started running or has turned the character, set to initial speed.
        if (xDir != 0 && (currentRunForce == 0 || hasTurned))
        {
            currentRunForce = startSpeed;
        }
        // player wants to run and we're not at max speed, speed us up.
        else if (xDir != 0 && currentRunForce < maxSpeed)
        {
            var newSpeed = currentRunForce + speedDiffForTimeDiff;
            var actualNewSpeed = newSpeed > maxSpeed ? maxSpeed : newSpeed;

            currentRunForce = actualNewSpeed;
        }
        // player wants to stop but we're still running, slow us down.
        else if (xDir == 0 && currentRunForce > 0f)
        {
            var newSpeed = currentRunForce - (speedDiffForTimeDiff * 5);
            var actualNewSpeed = newSpeed <= 0f ? 0f : newSpeed;

            currentRunForce = actualNewSpeed;
        }
    }
    
    float getJumpVelocity()
    {
        airState = GetAirState();

        if (isGrounded() && airState == AirState.LeavingGround && rb2D.velocity.y == 0)
        {
            return jumpForce;
        }
        else if (airState == AirState.SecondJump && yDir > 0f)
        {
            airState = AirState.SecondJumpNoKey;
            var adjustedForce = jumpForce - 20 * rb2D.velocity.y;
            return adjustedForce;
        }
        // If can hop up step, hop.
        else if (canStep() && xDir != 0)
        {
            return stepForce;
        }

        return 0f;
    }

    AirState GetAirState()
    {
        bool jumpPressed = yDir > 0f;

        if (isGrounded())
        {
            if (jumpPressed) return AirState.LeavingGround;
            return AirState.Grounded;
        }
        else
        {
            // Logic to handle jump/double jump
            switch (airState)
            {
                case AirState.LeavingGround:
                    return AirState.FirstJump;
                case AirState.FirstJump:
                    if (yDir == 0f) return AirState.FirstJumpNoKey;
                    return airState;
                case AirState.FallNoJump:
                case AirState.FirstJumpNoKey:
                    if (jumpPressed) return AirState.SecondJump;
                    return airState;
                case AirState.SecondJump:
                    if (!jumpPressed) return AirState.SecondJumpNoKey;
                    return airState;
                case AirState.SecondJumpNoKey:
                    return airState;
                default:
                    return AirState.FallNoJump;
            }
        }
    }

    bool canStep()
    {
        var hitRightStep = Physics2D.OverlapCircle(rStepCheck.position, stepRadius, whatIsGround);
        var hitLeftStep = Physics2D.OverlapCircle(lStepCheck.position, stepRadius, whatIsGround);

        var hitRightMax = Physics2D.OverlapCircle(rMaxStep.position, stepRadius, whatIsGround);
        var hitLeftMax = Physics2D.OverlapCircle(lMaxStep.position, stepRadius, whatIsGround);

        return ((hitRightStep != null && hitRightMax == null) || (hitLeftStep != null && hitLeftMax == null));
    }

    void UpdateSpriteState()
    {
        SetSpriteXDirection();

        if (!isGrounded())
        {
            animator.SetInteger("state", 2);
        }
        else if (xDir == 0f) {
            animator.SetInteger("state", 0);
        } else
        {
            animator.SetInteger("state", 1);
        }

        
    }

    void SetSpriteXDirection()
    {
        if (xDir > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (xDir < 0f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
