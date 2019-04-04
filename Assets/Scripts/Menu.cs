using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    //Load first level
    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    //Load main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
