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
    public bool isFacingRight = true;

    //Knight Code

    //Attack code
    [SerializeField] Transform attackPos;
    [SerializeField] LayerMask whatIsEnemies;
    [SerializeField] float attackRange;
    [SerializeField] int damage;


    //Hurt code
    [SerializeField] float bounceSpeed = 1f;
    [SerializeField] float hurtCooldown = 2f;


    //Slide code
    [SerializeField] BoxCollider2D slidingBoxCollider2D;
    [SerializeField] float boostSpeed = 10f;
    private bool canBoost = true;
    private float boostCooldown = 2f;
    public GameObject dashEffect, dashEffectPos;

    //Shoot code
    public GameObject projectile, gun;
    private GameObject projectileParent;


    //State
    bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    Health playerHealth;
    float gravityScaleAtStart;

    // Messages then methods
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<Health>();


        gravityScaleAtStart = myRigidbody.gravityScale;


        //Creates a parent if necessary
        projectileParent = GameObject.Find("Projectiles");

        if (!projectileParent)
        {
            projectileParent = new GameObject("Projectiles");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        Jump();
        FlipSprite();
        ClimbLadder();

        //Knight Code
        Attack();
        Crouching();
        Dash();
        CastSpell();
        Hurt();

        if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("isOnGround", true);
            return;
        }
    }
    private void Run()
    { 
          
        if (myAnimator.GetBool("isCrouching") == true && myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myRigidbody.velocity = new Vector2(0f, 0f);
            return;
        }
        
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
        //Code to prevent the player turning in the middle of a slide
        bool isSliding;
        isSliding = myAnimator.GetBool("isSliding");

        //If the player is moving horizontally
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        //Determine which way the player should face depending on his +/- x speed. As well as check to see if he's sliding
        if (playerHasHorizontalSpeed && isSliding == false)
        {
            // reverse the current scaling if the x axis
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
            //Determine if the player should face left or right. This is checking if the player's velocity is positive or negative. 
            if ((Mathf.Sign(myRigidbody.velocity.x) > Mathf.Epsilon))
                isFacingRight = true;
            else if ((Mathf.Sign(myRigidbody.velocity.x) < Mathf.Epsilon))
                isFacingRight = false;
        }
    }

    //If the player is touching either an enemy or a hazard, call die method and change player sprite
    private void Hurt()
    {
        //This is the amount of time that needs to pass before the player can take damage again
        hurtCooldown -= Time.deltaTime;
        if(hurtCooldown < 0)
        {
            hurtCooldown = 0;
        }
        //If player collides with an enemy or hazard and the hurt cooldown is 0
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")) && hurtCooldown <= 0f)
        {
            hurtCooldown = 1f; //Set the time they can't get hurt again
            StartCoroutine(HurtBounce(0.2f)); //Code to push the player back after taking a hit
            playerHealth.health -= 1;
            if(playerHealth.health <= 0)
            {
                Die();
            }

        }

    }

    IEnumerator HurtBounce(float bounceDur) //Coroutine with a single input of a float called boostDur, which we can feed a number when calling
    {
        myAnimator.SetTrigger("TakeDamage");
        float timeLength = 0; //create float to store the time this coroutine is operating
        isAlive = false; //Make it so player can't move while flying back
        //canBoost = false; //set canBoost to false so that we can't keep boosting while boosting


        if (isFacingRight)
        {
            while (bounceDur > timeLength) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
            {
                timeLength += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
                myRigidbody.velocity = new Vector2(-bounceSpeed, 0); //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
                yield return 0; //go to next frame
            }
        }
        else
        {
            while (bounceDur > timeLength) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
            {
                timeLength += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
                myRigidbody.velocity = new Vector2(bounceSpeed, 0); //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
                yield return 0; //go to next frame
            }
        }

        //myAnimator.SetBool("isSliding", false);
        isAlive = true; //Allow player to move again
        yield return 0; //Cooldown time for being able to boost again, if you'd like.
        //canBoost = true; //set back to true so that we can boost again.

    }
    //If the player is touching either an enemy or a hazard, call die method and change player sprite
    private void Die()
    {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
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

    //Used to the draw a circle in unity to let you know how big your attack range is
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
    //If any enemies are overlapping the circle hitbox, deal damage to them.
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
        //The joystick needs to be at least 75% pointing down to register as a crouch. If it was 0f, the player would crouch if they weren't 100% horizontal. 
        if (crouchingValue < -0.75f)
        {
            
            myAnimator.SetBool("isCrouching", true);
            myAnimator.SetBool("isRunning", false);

        }
        else
        {
            myAnimator.SetBool("isCrouching", false);
        }
    }
    
    //A slide move like in the OG megaman
    private void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump") && (myAnimator.GetBool("isCrouching") == true))
        {
            //Create a dash effect
            GameObject dash = Instantiate(dashEffect) as GameObject;
            //Set the spawn location to the location of the parent
            dash.transform.parent = projectileParent.transform;
            //Spawn the dash effect in the parent's dashPos location
            dash.transform.position = dashEffectPos.transform.position;


            StartCoroutine(Boost(0.4f)); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
        }
    }
    IEnumerator Boost(float boostDur) //Coroutine with a single input of a float called boostDur, which we can feed a number when calling
    {
        myAnimator.SetBool("isSliding", true);
        float timeLength = 0; //create float to store the time this coroutine is operating
        canBoost = false; //set canBoost to false so that we can't keep boosting while boosting

        
        if(isFacingRight)
        {
            while (boostDur > timeLength) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
            {
                timeLength += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
                myRigidbody.velocity = new Vector2(boostSpeed, 0); //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
                yield return 0; //go to next frame
            }
        }
        else
        {
            while (boostDur > timeLength) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
            {
                timeLength += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
                myRigidbody.velocity = new Vector2(-boostSpeed, 0); //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
                yield return 0; //go to next frame
            }
        }
        
        myAnimator.SetBool("isSliding", false);
        yield return new WaitForSeconds(boostCooldown); //Cooldown time for being able to boost again, if you'd like.
        canBoost = true; //set back to true so that we can boost again.
        
    }

    private void CastSpell()
    {
        if(CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            myAnimator.SetTrigger("Casting");
        }
    }

    private void FireSpell()
    {
        //Create a bullet based on whatever "projectile" the gameObject has assigned
        GameObject newProjectile = Instantiate(projectile) as GameObject;
        //Set the spawn location to the location of the parent
        newProjectile.transform.parent = projectileParent.transform;
        //Spawn the bullet in the parent's gun location
        newProjectile.transform.position = gun.transform.position;
    }
}
