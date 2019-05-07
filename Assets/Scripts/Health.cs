using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public int health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite FullHeart;
    public Sprite HalfHeart;
    public Sprite EmptyHeart;


    // Start is called before the first frame update
    void Start()
    {
        //Find all the heart containers in the canvas and assign them to the array. This should work on player respawn between deaths.
        for (int i = 0; i < hearts.Length; i++)
        {
            GameObject canvas = GameObject.Find("Heart Container" + i);
            hearts[i] = canvas.GetComponent<Image>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        ManageHealth();

    }

    private void ManageHealth()
    {
        //If player health is greater than maximum heart tanks, set it equal to current maximum
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }

        //For each heart tank in the array
        for (int i = 0; i < hearts.Length; i++)
        {
            //This will show how many full/empty heart containers the player should see based on how much health they have
            if (i < health)
            {
                hearts[i].sprite = FullHeart;
            }
            else
            {
                hearts[i].sprite = EmptyHeart;
            }

            //This will determine how many maximum heart containers the player will see
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else if (i >= numOfHearts)
            {
                hearts[i].enabled = false;
            }
        }
    }
}

    
