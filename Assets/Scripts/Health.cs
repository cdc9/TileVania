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
        for (int i = 0; i < hearts.Length; i++)
        {
            GameObject canvas = GameObject.Find("Heart Container"+ i);
            hearts[i] = canvas.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(health > numOfHearts)
        {
            health = numOfHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if(i < health)
            {
                hearts[i].sprite = FullHeart;
            }
            else
            {
                hearts[i].sprite = EmptyHeart;
            }

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
