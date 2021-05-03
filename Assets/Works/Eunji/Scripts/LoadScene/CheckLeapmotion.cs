using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CheckLeapmotion : MonoBehaviour
{
    public TextMeshProUGUI notConnected;
    public TextMeshProUGUI number;
    public UnityEngine.UI.Image circle;

    float counter = 5.0f;


    void Update()
    {
        notConnected.gameObject.SetActive(false);

        if (FindObjectOfType<GameManager>().isLeapMotionConnected)
        {
            if (SceneManager.GetActiveScene().name == "Loading")
                GetComponent<LoadSceneAuto>().GameStart();
            else if (SceneManager.GetActiveScene().name == "Loading2")
                GetComponent<LoadSceneAuto>().GameStart2();
            else // Tutorial_02
                GetComponent<LoadSceneAuto>().LoadNextScene();
        }

        else
        {
            notConnected.gameObject.SetActive(true);
            circle.gameObject.SetActive(true);

            if (counter > 0)
            {
                counter -= Time.deltaTime;
                number.text = ((int)counter).ToString();
            }
            else
            {
                GetComponent<LoadSceneAuto>().QuitGame();
                // SceneManager.LoadScene("Menu");
            }
        }
    }
}