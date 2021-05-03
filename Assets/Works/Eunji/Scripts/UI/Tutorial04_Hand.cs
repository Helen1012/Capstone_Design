using UnityEngine;

public class Tutorial04_Hand : MonoBehaviour
{
    private UnityEngine.UI.Image image;
    private float counter = 3.0f;
    public Sprite Paper;
    public Sprite Rock;

    void Start()
    {
        image = gameObject.GetComponent<UnityEngine.UI.Image>();
    }

    void Update()
    {
        if (counter > 1.5f)
        {
            // Rock
            counter -= Time.deltaTime;
        }
        else if(counter > 1.0f)
        {
            image.sprite = Paper;
            counter -= Time.deltaTime;
        }
        else
        {
             Debug.Log("else");
            image.sprite = Rock;
            counter = 3.0f;
        }
    }
}
