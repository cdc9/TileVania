using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{

    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float LevelExitSlowMoFactor = 0.2f;


    //This is a coroutine that calls the loadnextLevel method. Happens when the player hits the exit level object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine("LoadNextLevel");
    }

    IEnumerator LoadNextLevel()
    {
        //Slow down the game once the player hits level exit for a few seconds, load the next scene and then return time to normal
        Time.timeScale = LevelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        Time.timeScale = 1f;

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex +1 );
    }
}
