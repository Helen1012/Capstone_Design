using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFade : MonoBehaviour
{
    public Image img;
    public Image backgroundImg;

    public Sprite backgroundGray;

    private void Start()
    {
        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        
        if (fadeAway)
        {
            // fade from transparent to opaque
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            // fade from opaque to transparent
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }

            img.color = new Color(1, 1, 1, 0);

            backgroundImg.GetComponent<Image>().sprite = backgroundGray;
            GameObject.Find("Canvas").GetComponent<CTTDemoManager>().TitlePlay();
        }
    }
}