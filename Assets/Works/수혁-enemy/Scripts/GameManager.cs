using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Leap;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;


    // 게임종료 여부
    public bool isGameOver = false;

    /* ----------- Leap Motion ---------- */
    public Controller controller;
    private List<Finger> fingers;
    private GameObject checkHandCube;
    public bool isLeapMotionConnected = false;
    public bool isShoot = false;   // 총쏘기
    public bool isGrenade = false; // 수류탄
    public bool isLoading = false; // 장전

    /* -----------Player Win/Lose ---------- */

    private TextMeshProUGUI YouWin;
    private TextMeshProUGUI GameOver;

    // public bool isPlayerWin = false;

    public bool isPlayerWininFirst = false;
    public bool isPlayerWininSecond = false;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
            if (checkHandCube == null && GameObject.FindGameObjectWithTag("Cube"))
                checkHandCube = GameObject.FindGameObjectWithTag("Cube");

            if (YouWin == null && GameObject.Find("YouWin"))
            {
                YouWin = GameObject.Find("YouWin").GetComponent<TextMeshProUGUI>();
                YouWin.gameObject.SetActive(false);
            }

            if (GameOver == null && GameObject.Find("GameOver"))
            {
                GameOver = GameObject.Find("GameOver").GetComponent<TextMeshProUGUI>();
                GameOver.gameObject.SetActive(false);
            }

        // check Leap Motion connection
        StartCoroutine("checkHand");
    }

    IEnumerator checkHand()
    {
        controller = new Controller(1);

        if (controller == null)
        {
            controller = new Controller(1);
            Debug.Log("controller is null");
        }

        if (!controller.IsConnected)
        {
            controller.StartConnection();

            Invoke("QuitGame", 20.0f); // 20초 이내 연결되지 않으면 스크립트 종료
            isLeapMotionConnected = false;
        }
        else
        {
            CancelInvoke("QuitGame");
            isLeapMotionConnected = true;

            // if(SceneManager.GetActiveScene().name == "Loading")
            //     SceneManager.LoadScene("PlayMode"); // 수정하기
        }
        
        if (checkHandCube != null)
        {
            Hand hand = new Hand();
            Hand previous_hand = new Hand();

            Vector handPalmPosition = new Vector();
            Vector prehandPalmPosition = new Vector();

            Frame frame = controller.Frame();           // The latest frame
            Frame previous = controller.Frame(1);       // The previous frame

            for (int h = 0; h < frame.Hands.Count; h++)
            {
                hand = frame.Hands[0]; // 현재 나타나는 손
                handPalmPosition = hand.PalmPosition;   // 현재 손의 위치

                if (!previous.Hands.Any()) continue;
                previous_hand = previous.Hands[0]; // 이전 프레임에 나타나는 손
                prehandPalmPosition = previous_hand.PalmPosition;   // 이전 프레임의 손의 위치

                fingers = hand.Fingers; // 현재 손가락의 개수

                int _extendedFingers = getExtendedFingers();    // 함수를 호출하여 펼쳐진 손가락의 개수를 확인한다
                Debug.Log("_extendedFingers: " + _extendedFingers);

                isShoot = false; isGrenade = false; isLoading = false;

                // [Conditions to 'Shoot']
                //  1. Two straight fingers
                //  2. Hands moving from top to bottom

                // Debug.Log("x : " + (handPalmPosition.x - prehandPalmPosition.x));
                // Debug.Log("y : " + (handPalmPosition.y - prehandPalmPosition.y));
                //Debug.Log("z : " + (handPalmPosition.z - prehandPalmPosition.z));

                if (_extendedFingers == 2 && System.Math.Abs(handPalmPosition.y - prehandPalmPosition.y) > 5)
                {
                    checkHandCube.GetComponent<MeshRenderer>().material.color = Color.red;
                    isShoot = true;
                }
                // [Condition for changing weapons]
                //  1. Hands moving from side to side (swipe)
                else if (System.Math.Abs(handPalmPosition.x - prehandPalmPosition.x) > 10
                    && System.Math.Abs(handPalmPosition.y - prehandPalmPosition.y) > 3)
                {
                    checkHandCube.GetComponent<MeshRenderer>().material.color = Color.green;
                    isGrenade = true;
                }
                // [Condition to Load]
                //  1. Gripped left hand
                else if (hand.GrabStrength == 1 && _extendedFingers == 0)
                {
                    checkHandCube.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    isLoading = true;
                }
                else
                {
                    checkHandCube.GetComponent<MeshRenderer>().material.color = Color.black;
                }
            } // end for
            yield return null;
        } // end if
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game

        //UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private int getExtendedFingers()
    {
        int extendedFingers = 0;

        for (int f = 0; f < fingers.Count; f++)
        {
            Finger digit = fingers[f];
            if (digit.IsExtended)
                extendedFingers++;
        }
        return extendedFingers;
    }

    public void playerWin()
    {
        /* You Win UI 띄우기 */
        if (!YouWin) return;
        YouWin.gameObject.SetActive(true);
        GameObject.Find("Main Camera").GetComponent<PlayGlitchEffect>().Play();
        StartCoroutine(ShowLevelTimer());
        
        // isPlayerWin = true;
        if (SceneManager.GetActiveScene().name == "PlayMode") isPlayerWininFirst = true;
        else if(SceneManager.GetActiveScene().name == "PlayMode2") isPlayerWininSecond = true;
        Debug.Log("isPlayerWininFirst : " + isPlayerWininFirst);
    }

    public void playerLose()
    {
        /* Game over UI 띄우기 */
        if (!GameOver) return;

        GameOver.gameObject.SetActive(true);
        GameObject.Find("Main Camera").GetComponent<PlayGlitchEffect>().Play();
        StartCoroutine(ShowLevelTimer());
        
        // isPlayerWin = false;
        if (SceneManager.GetActiveScene().name == "PlayMode") isPlayerWininFirst = false;
        else if(SceneManager.GetActiveScene().name == "PlayMode2") isPlayerWininSecond = false;
    }

    IEnumerator ShowLevelTimer()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        SceneManager.LoadScene("Level");
    }
}
