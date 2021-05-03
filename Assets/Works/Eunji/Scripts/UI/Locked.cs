using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Locked : MonoBehaviour
{
    public Animator anim;
    private Image img;
    private bool isFadeOut;

    void Start()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        
        if (FindObjectOfType<GameManager>().isPlayMode2Played)
            img.color = new Color(1, 1, 1, 0);
        else if (FindObjectOfType<GameManager>().isPlayerWininFirst)
            StartCoroutine(FadeOut());
        else
            anim.Play("LockAnim");
    }
    IEnumerator FadeOut()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
}
