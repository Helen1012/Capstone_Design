﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CheckLeapmotion : MonoBehaviour
{
    public Text notConnected;
    public UnityEngine.UI.Image circle;

    private void Start()
    {
        // leapmotion = new Controller();
        //notConnected = GameObject.Find("NotConnected").GetComponent<Text>();
        //circle = GameObject.Find("Circle").GetComponent<UnityEngine.UI.Image>();

        // if (!leapmotion.IsConnected)
        // {
        //     leapmotion = new Controller(1);
        // }

        notConnected.gameObject.SetActive(false);
        circle.gameObject.SetActive(false);
    }

    public void onClickLikeButton()
    {
        Debug.Log("Click!");
        circle.gameObject.SetActive(true);

        if (FindObjectOfType<GameManager>().controller.IsConnected)
            Invoke("LoadNextScene", 2.0f);
        else
        {
            notConnected.gameObject.SetActive(true);
            circle.gameObject.SetActive(false);
        }
    }

    private void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }
}