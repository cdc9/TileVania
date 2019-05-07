using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{

    //Blue hearts
    public int mana;
    public int numOfBlueHearts;
    [SerializeField] float timeToRegenMana = 5f;

    public Image[] blueHearts;
    public Sprite BlueFullHeart;
    public Sprite BlueHalfHeart;
    public Sprite BlueEmptyHeart;
    public bool halfHeartBool = false;
    public bool halfHeartSeen;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < blueHearts.Length; i++)
        {
            GameObject canvas = GameObject.Find("Blue Heart Container" + i);
            blueHearts[i] = canvas.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageMana();
        RegenMana();

    }

    private void ManageMana()
    {
        if (mana > numOfBlueHearts)
        {
            mana = numOfBlueHearts;
        }

        for (int i = 0; i < blueHearts.Length; i++)
        {
            if (i < mana)
            {
                blueHearts[i].sprite = BlueFullHeart;
            }

            else
            {
                blueHearts[i].sprite = BlueEmptyHeart;
            }

            if (halfHeartBool == true)
            {
                int newHalfHeartNum = mana;
                blueHearts[newHalfHeartNum].sprite = BlueHalfHeart;
            }
            if (i < numOfBlueHearts)
            {
                blueHearts[i].enabled = true;
            }
            else if (i >= numOfBlueHearts)
            {
                blueHearts[i].enabled = false;
            }
        }
    }

    private void RegenMana()
    {
        if (mana < numOfBlueHearts)
        {
            timeToRegenMana -= Time.deltaTime;
        }
        if(timeToRegenMana < 0)
        {
            timeToRegenMana = 5f;
            mana += 1;
            halfHeartBool = false;
        }
        if (timeToRegenMana < 2.5)
        {
            halfHeartBool = true;
        }
    }
}

