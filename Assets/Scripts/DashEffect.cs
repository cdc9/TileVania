using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    private bool isKnightFacingRight;
    Knight knight;

    // Start is called before the first frame update
    void Start()
    {
        knight = FindObjectOfType<Knight>();
        isKnightFacingRight = knight.isFacingRight;

        //Determine when direction the projectile should be facing when it first appears
        if (isKnightFacingRight)
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
        
    }

    private void End()
    {
        Destroy(gameObject);
    }
}
