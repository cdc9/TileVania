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


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        enemyMovement = GetComponent<EnemyMovement>();

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
        }
    }

    public void TakeDamage(int damage)
    {
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        health -= damage;
        playerHitMe = true;
        if (hurtCooldown <= 0)
        {
            hurtCooldown = .5f;
            enemyMovement.moveSpeed = 0f;
        }
        
    }
}
