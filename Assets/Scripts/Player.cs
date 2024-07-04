using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    /*
        Movement and Jumps Start
    */
    [Header("Movement details")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;

    private bool canDoubleJump = true;
    private bool isInAir = false;
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
        HandleCollision();
        HandleInput();
        HandleFlip();
        HandleMovements();
        HandleAnimations();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, new Vector2(this.transform.position.x, this.transform.position.y - groundCheckDistance));
    }


    private void HandleMovements()
    {
        this.rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y));
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", this.rb.velocity.x);
        anim.SetFloat("yVelocity", this.rb.velocity.y);
        anim.SetBool("isGround", this.isGround);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerJumpController();
        }
    }

    private void HandleCollision()
    {
        // Check for ground
        isGround = Physics2D.Raycast(this.transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isInAir = !isGround;
    }

    private void PlayerJumpController() {
        if(isGround) {
            PlayerJump();
        }
        else if(!isGround && isInAir) {
            PlayerDoubleJump();
        } 

    }
    private void PlayerJump()
    {
        this.rb.velocity = new Vector2(this.rb.velocity.x, jumpForce);
    }

    private void PlayerDoubleJump() {
        PlayerJump();
        canDoubleJump = false;
    }

    private void Flip() {
        this.transform.Rotate(new Vector3(0, 180, 0));
        currentFacingRight = !currentFacingRight;

    }
    private void HandleFlip()
    {
        if(this.rb.velocity.x > 0 && !currentFacingRight || this.rb.velocity.x < 0 && currentFacingRight) {
            Flip();
        }
    }



}
