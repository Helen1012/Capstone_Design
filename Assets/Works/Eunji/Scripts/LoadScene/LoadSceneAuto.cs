﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAuto : MonoBehaviour
{
    // ---- Tutorial_01 ----
    // If the Arrow Button is clicked, 'Tutorial_02' is loaded.


    // ---- Tutorial_02 ----
    // After the Yes Button is clicked, if Leapmotion controller is connected, 'Tutorial_03' is loaded.


    // ---- Tutorial_03 ----
    // If Like Button is clicked, go to 'Tutorial_04'.
    // Test Leapmotion Hand's Action
    // 1) Loading  2) Grenade  3) Shooting

    // If Dislike Button is clicked, run the LoadGameScene() and go to 'Tutorial_07'.
    // Pass the Leapmotion Test

    public void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
        StartCoroutine("timer");
    }
    
    public void LoadGameScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 4); 
        StartCoroutine("timer");
    }

    public void GameStart() {

    }

    public void TutorialStart() {

    }
    
    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game

        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(5.0f);
    }
}
