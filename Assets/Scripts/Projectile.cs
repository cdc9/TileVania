﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Configs
    public float bulletSpeed;
    public int damage = 3;

    //Components 
    Rigidbody2D myRigidbody;
    BoxCollider2D myBoxCollider;
    Knight knight;
    [SerializeField] GameObject explosionEffect;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize the components
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        knight = FindObjectOfType<Knight>();

        //Determine when direction the projectile should be facing when it first appears
        if (knight.isFacingRight)
        {
            // reverse the current scaling if the x axis
            transform.localScale = new Vector2(1f, 1f);
        }
        else
        {
            // reverse the current scaling if the x axis
            transform.localScale = new Vector2(-1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Fly();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Trigger the explosion effect after colliding with something
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //If it hits an enemy, damage it
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
            collision.GetComponent<Enemy>().TakeDamage(damage);

        Destroy(gameObject);
       
    }
    //Move projectile and determine with direction it should be flying in
    private void Fly()
    {
        if(knight.isFacingRight)
        {
            myRigidbody.velocity = new Vector2(bulletSpeed, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(-bulletSpeed, myRigidbody.velocity.y);
        }
        
    }
}
