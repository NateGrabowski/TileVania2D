using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 4f;
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] float climbSpeed = 4f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 20f);
    [SerializeField] GameObject myBullet;
    [SerializeField] Transform gun;
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;


    bool playerAlive = true;
    float gravity = 0f;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        //myBullet = GetComponent<GameObject>();
        gravity += myRigidbody.gravityScale;

    }

    void Update()
    {
        if (playerAlive)
        {
            Run();
            FlipSprite();
            ClimbLadder();
            Died();
        }
    }

    private void Died()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")) || myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            playerAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();

        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);

        }
    }

    private void Run()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        if (playerHasHorizontalSpeed) //Changes the animation state from Idle to Run if running
        {
            myAnimator.SetBool("isRunning", true);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);

        }
    }

    void OnFire(InputValue val)
    {
        if (val.isPressed && playerAlive)
        {
            Instantiate(myBullet, gun.position, transform.rotation);
        }
    }

    void OnMove(InputValue val)
    {
        moveInput = val.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue val)
    {
        if (val.isPressed && playerAlive)
        {
            if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) //If player is touching ground allow jump
            {
                myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    private void ClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) //If player is touching ladder allow jump
        {
            myRigidbody.gravityScale = 0f;
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
        }
        else
        {
            myRigidbody.gravityScale = gravity;
            myAnimator.SetBool("isClimbing", false);
        }
    }
}
