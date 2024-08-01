using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*
        Movement and Jumps Start
    */
    [Header("Movement details")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float xInput;
    [SerializeField] private float yInput;

    [SerializeField]private bool canDoubleJump = true;
    [SerializeField] private bool isInAir = false;
    [SerializeField] private float doubleJumpForce;

    /* Wall Slide */
    [SerializeField] private float wallDetectionDistance;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private bool isWallDetected = false;
    [SerializeField] private float slidingSpeed = 0.5f; 
    [SerializeField] private float slidingHoldSpeed = 5f; 

    /* Wall Jump */
    [SerializeField] private bool isWallJumping = false;
    [SerializeField] private Vector2 wallJumpForce;

    [SerializeField] private float wallJumpCooldownDuration = 0.6f;   

    /*
        Movement and Jumps End
    */

    /* Player Flip Start*/
    private bool currentFacingRight = true; // True for right, False for left

    /* Player Flip End*/


    /*
        PlayerComponents Start
    */
    private Rigidbody2D rb;
    private Animator anim;
    /*
        PlayerComponents End
    */

    /* PlayerCollision Start*/
    [SerializeField] private bool isGround = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;

    /* PlayerCollision End*/

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        HandleAirCondition();
        
        HandleInput();
        HandleFlip();
        HandleMovements();
        HandleWallSlide();
        HandleCollision();
        HandleAnimations();

    }


    private void PlayerKnockback() {
        
        anim.SetTrigger("knockback");
        rb.velocity = new Vector2(-xInput * 5f, 10f);
        

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(this.transform.position, new Vector2(this.transform.position.x + (wallDetectionDistance* (currentFacingRight ? 1: -1)), this.transform.position.y));
    }


    private void HandleMovements()
    {
        
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        this.rb.AddForce(new Vector2(xInput * movementSpeed, rb.velocity.y));
    }
    private void PlayerWallJumping() {
        rb.velocity = new Vector2(wallJumpForce.x * (currentFacingRight? -1 : 1), wallJumpForce.y);
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpCooldown());
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", this.rb.velocity.x);
        anim.SetFloat("yVelocity", this.rb.velocity.y);
        anim.SetBool("isGround", this.isGround);
        anim.SetBool("isWallDetected", this.isWallDetected);
    }

    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.K)){
            PlayerKnockback();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerJumpController();
        } else if(Input.GetKey(KeyCode.DownArrow) && isWallDetected && !isGround) {
            this.rb.velocity = new Vector2(this.rb.velocity.x, this.rb.velocity.y - slidingHoldSpeed);
        }
    }

    private void HandleAirCondition() {

        if(isGround && isInAir) {
            HandleLanding();
        }

        if(!isInAir && !isGround) {
            isInAir = true;
        }

    }

    private void HandleLanding() {
        isInAir = false;
        canDoubleJump = true;
    }

    private void HandleCollision()
    {
        // Check for ground
        isGround = Physics2D.Raycast(this.transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallDetected = Physics2D.Raycast(this.transform.position, (Vector2.right * (currentFacingRight ? 1 : -1)), wallDetectionDistance, wallMask );
    }
    private void PlayerJumpController() {
        if(isGround ) {
            PlayerJump();
        }
        else if(isWallDetected) {
            PlayerWallJumping();
        }
        else if(canDoubleJump) {
            PlayerDoubleJump();
        } 
    }

    private IEnumerator WallJumpCooldown() {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpCooldownDuration);
        isWallJumping = false;

    }
    private void PlayerJump()
    {
        this.rb.velocity = new Vector2(this.rb.velocity.x, jumpForce);
    }

    private void PlayerDoubleJump() {
        this.rb.velocity = new Vector2(this.rb.velocity.x, doubleJumpForce);
        canDoubleJump = false;
    }

    private void Flip() {
        this.transform.Rotate(new Vector3(0, 180, 0));
        currentFacingRight = !currentFacingRight;
    }
    private void HandleFlip()
    {
        if(xInput > 0 && !currentFacingRight || xInput < 0 && currentFacingRight) {
            Flip();
        }
    }

    private void HandleWallSlide() {
        if (isWallDetected && this.rb.velocity.y <= 0 ) {
            this.rb.velocity = new Vector2(this.rb.velocity.x,
             this.rb.velocity.y * slidingSpeed);
        }
    }
}
