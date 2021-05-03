using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAuto : MonoBehaviour
{
    /* Scene Manager */

    // ---- Start ----
    // In Start Scene, it displayes INU logo and our title.
    // After 9 seconds to start, Menu Scene is loaded. ( LoadMenu() )

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


    public void LoadMenu(){
        
        int index = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadMenuTimer(index));
    }

    public void LoadMenuIfNotConnected(){
        // SceneManager.LoadScene("Menu");
        // SceneManager.LoadScene(1);
        StartCoroutine(LoadMenuIfNotConnectedTimer());
    }

    public void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadNextSceneTimer(index));
    }
    
    public void SkipTutorial()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(SkipTutorialTimer(index));
    }

    public void LoadingStart(){
        SceneManager.LoadScene("Loading");
    }
    public void LoadingStart_02(){
        SceneManager.LoadScene("Loading2");
    }

    public void GameStart() {
        StartCoroutine(PlayModeTimer());
    }
    public void GameStart2() {
        StartCoroutine(PlayModeTimer2());
    }

    public void TutorialStart() {
        SceneManager.LoadScene("Tutorial_01");
    }

    IEnumerator LoadMenuTimer(int index)
    {
        yield return new WaitForSecondsRealtime(9.0f);
        SceneManager.LoadScene(index + 1);
    }

    IEnumerator LoadMenuIfNotConnectedTimer()
    {
        yield return null;
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadNextSceneTimer(int index)
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("loadnextScene");
        SceneManager.LoadScene(index + 1);
    }
    IEnumerator SkipTutorialTimer(int index)
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(index + 4);
    }

    IEnumerator PlayModeTimer()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("PlayMode");
    }

    IEnumerator PlayModeTimer2()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("PlayMode2");
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game

        UnityEditor.EditorApplication.isPlaying = false;
#else

        System.Diagnostics.Process.GetCurrentProcess().Kill();
        // Application.Quit();
#endif
    }
    
}
