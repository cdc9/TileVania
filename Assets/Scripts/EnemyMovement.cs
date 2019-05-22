using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    //Configs
    public float moveSpeed = 5f;

    //State
    bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidBody;
    BoxCollider2D myBoxCollider;
    Enemy enemy;

    // Messages then method
    // Start is called before the first frame update
    void Start() 
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsFacingRight())
        {
            myRigidBody.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            myRigidBody.velocity = new Vector2(-moveSpeed, 0f);
        }
        
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        if (collision.tag == "Ground")  
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && enemy.playerHitMe == false)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
        }
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && enemy.playerHitMe == true)
        {
            enemy.playerHitMe = false;
        }


    }
}
