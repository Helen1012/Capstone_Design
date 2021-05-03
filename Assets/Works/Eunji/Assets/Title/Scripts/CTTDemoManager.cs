using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTTDemoManager : MonoBehaviour {

    [Header("STYLE OBJECTS")]
    public List<GameObject> objects = new List<GameObject>();

    [Header("STYLE PARENTS")]
    public List<GameObject> panels = new List<GameObject>();

    [Header("SETTINGS")]
    public int currentPanelIndex = 0;

    public AudioSource audiosource;

    // [Header("PANEL ANIMS")]
    private string panelFadeIn = "Panel Open";
    private string panelFadeOut = "Panel Close";
    private string styleExpand = "Expand";

    private GameObject currentPanel;
    private GameObject nextPanel;

    private Animator currentPanelAnimator;
    private Animator styleAnimator;

    void Start(){
        currentPanel = panels[currentPanelIndex];
        currentPanel.SetActive(false);
    }

    public void TitlePlay()
    {
        currentPanel.SetActive(true);
        currentPanelAnimator = currentPanel.GetComponent<Animator>();
        currentPanelAnimator.Play(panelFadeIn);
        StartCoroutine(audiosourcePlay());
    }

    public void PanelAnim(int newPanel)
    {
        if (newPanel != currentPanelIndex)
        {
            currentPanel = panels[currentPanelIndex];

            currentPanelIndex = newPanel;
            nextPanel = panels[currentPanelIndex];

            currentPanelAnimator = currentPanel.GetComponent<Animator>();

            currentPanelAnimator.Play(panelFadeOut);

            styleAnimator = currentPanel.GetComponent<Animator>();
            styleAnimator.Play(styleExpand);
        }    
    }

    IEnumerator audiosourcePlay(){
        yield return new WaitForSecondsRealtime(0.3f);
        audiosource.Play();
    }
}
