using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeSpeed = 0.01f;
    private float totalTime = 0f;
    private float fills;
    private float bt = 0.1f;
    public Image progressBar;
    public GameObject Spotlight;
    public GameObject Pointlight;
    public GameObject Fleshlight;
    public float delayTime = 20.0f;
    public bool lightmanager = false;
    public AudioSource audioSound;
    public AudioClip Sirensound;

    void Start()
    {
        Fleshlight.SetActive(false);
        progressBar.fillAmount = 0;
    }


    void Update()
    {
        if (fills >= 0.3f && lightmanager == false)
        {
            AudioStart();
        }
        if (fills >= 0.4f && lightmanager == false)
        {
            LightManager();
            
          

        }

        if (progressBar.fillAmount >= 1)
        {
            FindObjectOfType<GameManager>().playerWin();
            enabled = false;
        }
        else
        {
            totalTime += Time.deltaTime * 1; /* set up the speed */

            fills = totalTime * timeSpeed;
            progressBar.fillAmount = fills;
        }
    }

    void AudioStart()
    {
        audioSound.clip = Sirensound;
        audioSound.Play();
    }

    void LightManager()
    {
        lightmanager = true;

        StartCoroutine(Blinking());
        StartCoroutine(Blinktime());
        bt += 0.2f;
        StartCoroutine(Blinking());
        StartCoroutine(Blinktime());
        bt += 0.6f;
        StartCoroutine(Blinking());
        StartCoroutine(FleshLightOn());
        StartCoroutine(WaitForIt());
    }
    IEnumerator Blinking()
    {
        yield return new WaitForSeconds(bt);
        Spotlight.SetActive(false);
        Pointlight.SetActive(false);
    }
    IEnumerator FleshLightOn()
    {
        yield return new WaitForSeconds(1.2f);
        Fleshlight.SetActive(true);

    }
    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(delayTime);

        Fleshlight.SetActive(false);
        Spotlight.SetActive(true);
        Pointlight.SetActive(true);
    }
    IEnumerator Blinktime()
    {
        yield return new WaitForSeconds(bt * 2);
        Spotlight.SetActive(true);
        Pointlight.SetActive(true);
    }

}
