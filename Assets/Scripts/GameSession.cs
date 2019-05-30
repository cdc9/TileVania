using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{

    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;

    [SerializeField] Text livesText;
    [SerializeField] Text scoresText;

    // When this is called for the first time, check to see if there are other game session scripts running. If there are, delete this new one. This is to make sure only 1 game session persists between player lives and reloading scene
    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded; //Check to see if the player in on the win screen in order to remove the HUD


    }
    // Start is called before the first frame update
    void Start()
    {
        livesText.text = playerLives.ToString();
        scoresText.text = score.ToString();
        string sceneName = "Win Screen";
        if (SceneManager.GetActiveScene().name == sceneName) //could compare Scene.name instead
        {
            Destroy(this); //change as appropriate
        }
    }

    //Add to the player score
    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoresText.text = score.ToString();
    }

    //A public method other classes can access in order to make the player die. OR reset the game session if all lives are lost
    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    //Subtract one life from the player and reload the level
    private void TakeLife()
    {
        playerLives--;
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        livesText.text = playerLives.ToString();
    }

    //Restart the game to the main menu
    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    //Check if player is on final screen to remove HUD
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Win Screen")
        {
            Destroy(gameObject);
            Debug.Log("I am inside the if statement");
        }
    }
}
