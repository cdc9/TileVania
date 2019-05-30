using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public bool playerHitMe = false;
    [SerializeField] GameObject bloodEffect;
    [SerializeField] float hurtCooldown = .2f;

    public EnemyMovement enemyMovement;
    //public Animator camAnim;

    // Cached component references
    Rigidbody2D myRigidBody;
    BoxCollider2D myBoxCollider;
    CapsuleCollider2D myCapsuleCollider;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }

        hurtCooldown -= Time.deltaTime;
        if (hurtCooldown < 0)
        {
            hurtCooldown = 0;
            enemyMovement.moveSpeed = 3f;
            playerHitMe = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (hurtCooldown <= 0)
        {
            Instantiate(bloodEffect, transform.position, Quaternion.identity);
            playerHitMe = true;
            health -= damage;
            hurtCooldown = .25f;
            enemyMovement.moveSpeed = 0f;
        }
        
    }
}
