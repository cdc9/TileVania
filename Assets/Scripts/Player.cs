using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class Player : MonoBehaviour
{
    //Configs
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25, 25);

    //State
    bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    float gravityScaleAtStart;

    // Messages then methods
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();

        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive) { return; }

        Run();
        Jump();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void Run()
    {
        //NOTE: Go to project settings -> Input -> gravity to adjust how quickly the value of GetAxis drops to zero after letting go. Value 3 makes it feel slippery/laggy. Value 10 Stops almost immediately. Alternitively, user GetAxisRaw for -1,0,1 values. 
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); //value is between -1 and +1 
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y); // vector2(x,y) where x is horizontal movement, and y is whatever the current y movement the player is going right now. if you put 0, player would stop all y axis movement
        myRigidbody.velocity = playerVelocity; // set the new velocity

        //Determine if the player''s running animation should be playing. If velocity is 0, stop running.
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    private void ClimbLadder()
    {
        //Check to see if the player touching the ladder or not first
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        {
            myAnimator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart; //When not touching ladder, set gravity to normal
            return; 
        }


        //Allow the player to climb
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical"); //value is between -1 and +1 
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed); // vector2(x,y) where x is horizontal movement, and y is whatever the current y movement the player is going right now. if you put 0, player would stop all y axis movement
        myRigidbody.velocity = climbVelocity; // set the new velocity
        myRigidbody.gravityScale = 0f; //Player stops sliding down ladder while on it.

        //Determine if the player''s running animation should be playing. If velocity is 0, stop running.
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {
        //Check to see if the player touching the ground or not first
        if(!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))){ return; }

        //Allow the player to jump
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;

        }
    }

    private void FlipSprite()
    {
        //If the player is moving horizontally
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; 
        if(playerHasHorizontalSpeed)
        {
            // reverse the current scaling if the x axis
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    //If the player is touching either an enemy or a hazard, call die method and change player sprite
    private void Die()
    {
        if(myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            myRigidbody.velocity = deathKick; 
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }

    }

    
}
