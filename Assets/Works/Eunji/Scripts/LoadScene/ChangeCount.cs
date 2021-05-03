using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCount : MonoBehaviour
{
    public GameObject checkHandAction;
    public Text countDown;
    public int count;
    private bool isPresentColorSameAsCurrentColor = false;

    private Color yellow = Color.yellow;
    private Color red = Color.red;
    private Color green = Color.green;
    private Color Black = Color.black;
    private Color mainColor = new Color();

    // Start is called before the first frame update
    void Start()
    {
        string SceneName = SceneManager.GetActiveScene().name;

        switch (SceneName)
        {
            case "Tutorial_04":
                {
                    mainColor = yellow; break;
                }
            case "Tutorial_05":
                {
                    mainColor = green; break;
                }
            case "Tutorial_06":
                {
                    mainColor = red; break;
                }
            default: break;
        }

        countDown.text = count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPresentColorSameAsCurrentColor == false && checkHandAction.GetComponent<Renderer>().material.color == mainColor)
        {
            countDown.text = (count--).ToString();
            isPresentColorSameAsCurrentColor = true;
            StartCoroutine("timer");
        }
        else if(checkHandAction.GetComponent<Renderer>().material.color != mainColor)
            isPresentColorSameAsCurrentColor = false;
        else if(count < 1)
            Invoke("LoadNextScene", 2.0f);
    }

    private void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(10.0f);
    }
}
