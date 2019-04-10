using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Knight : MonoBehaviour
{
    //Configs
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25, 25);

    //Knight Code
    [SerializeField] Transform attackPos;
    [SerializeField] LayerMask whatIsEnemies;
    [SerializeField] float attackRange;
    [SerializeField] int damage;

    //Slide code
    [SerializeField] float slideSpeed = 10f;
    private float slideTime = 0f;
    public float startSlideTime = 1f;
    [SerializeField] BoxCollider2D slidingBoxCollider2D;

    //New slide code
    private Vector2 boostSpeed = new Vector2(10, 0);
    private bool canBoost = true;
    private float boostCooldown = 2f;


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
        //slideTime = startSlideTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        //Jump();
        FlipSprite();
        ClimbLadder();
        Attack();
        Crouching();
        //Slide();
        Dash();
        Die();

        if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isOnGround", true);
            return;
        }
    }
    private void Run()
    { 
        /*  
        if (myAnimator.GetBool("isCrouching") == true)
        {
            myRigidbody.velocity = new Vector2(0f, 0f);
            return;
        }
        */
        //NOTE: Go to project settings -> Input -> gravity to adjust how quickly the value of GetAxis drops to zero after letting go. Value 3 makes it feel slippery/laggy. Value 10 Stops almost immediately. Alternitively, user GetAxisRaw for -1,0,1 values. 
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); //value is between -1 and +1 
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y); // vector2(x,y) where x is horizontal movement, and y is whatever the current y movement the player is going right now. if you put 0, player would stop all y axis movement
        myRigidbody.velocity = playerVelocity; // set the new velocity

        //Determine if the player''s running animation should be playing. If velocity is 0, stop running.
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
        //Check to see if the player is touching the ground

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
        
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isOnGround", false);
            return;
        }
        
        //Allow the player to jump
        if (CrossPlatformInputManager.GetButtonDown("Jump") && (myAnimator.GetBool("isCrouching") == false))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;
            myAnimator.SetTrigger("Jumping");
            myAnimator.SetBool("isOnGround", false);
        }
    }

    private void FlipSprite()
    {
        //If the player is moving horizontally
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            // reverse the current scaling if the x axis
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    //If the player is touching either an enemy or a hazard, call die method and change player sprite
    private void Die()
    {
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            myRigidbody.velocity = deathKick;
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }

    }

    //Knight Code

    private void Attack()
    {
        //Check to see if the player touching the ground or not first
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            myAnimator.SetTrigger("Attacking");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void DealDamage()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    private void Crouching()
    {
        //Check to see if the player touching the ground or not first
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        //Allow the player to crouch
        float crouchingValue = CrossPlatformInputManager.GetAxis("Vertical"); //value is between -1 and +1
        //bool crouchAbs = Mathf.Abs(crouchingValue) > Mathf.Epsilon;
        if (crouchingValue < 0f)
        {
            
            myAnimator.SetBool("isCrouching", true);

        }
        else
        {
            myAnimator.SetBool("isCrouching", false);
        }
    }

    private void Slide()
    {
        //Check to see if the player touching the ground or not first
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isOnGround", false);
            return;
        }
        
        //Allow the player to jump
        if (CrossPlatformInputManager.GetButtonDown("Jump") && (myAnimator.GetBool("isCrouching") == true))
        {
            
            if (slideTime <= 0)
            {
                slideTime = startSlideTime;
                myRigidbody.velocity = Vector2.zero;
                myAnimator.SetBool("isSliding", false);
            }
            else
            {

                Debug.Log("Slide now!!");
                myAnimator.SetBool("isSliding", true);
                while (slideTime > 0)
                {
                    slideTime -= Time.deltaTime;
                    Vector2 slideVelocityToAdd = new Vector2(slideSpeed, 0f);
                    myRigidbody.velocity += slideVelocityToAdd;
                    //myRigidbody.velocity = new Vector2(slideSpeed, myRigidbody.velocity.y);
                }
            }

                /*
                Vector2 slideVelocityToAdd = new Vector2(slideSpeed, 0f);
                myRigidbody.velocity += slideVelocityToAdd;
                myAnimator.SetTrigger("Sliding");
                */
        }
        
    }
    
    private void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump") && (myAnimator.GetBool("isCrouching") == true))
        {
            StartCoroutine(Boost(0.2f)); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
        }
    }
    IEnumerator Boost(float boostDur) //Coroutine with a single input of a float called boostDur, which we can feed a number when calling
    {
        myAnimator.SetBool("isSliding", true);
        float timeLength = 0; //create float to store the time this coroutine is operating
        canBoost = false; //set canBoost to false so that we can't keep boosting while boosting

        while (boostDur > timeLength) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
        {
            timeLength += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
            myRigidbody.velocity = boostSpeed; //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
            yield return 0; //go to next frame
        }
        yield return new WaitForSeconds(boostCooldown); //Cooldown time for being able to boost again, if you'd like.
        canBoost = true; //set back to true so that we can boost again.
        myAnimator.SetBool("isSliding", false);
    }
}
