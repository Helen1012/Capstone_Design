using UnityEngine;
using TMPro;

public class PlayAnimInLock : MonoBehaviour
{
    public Animator anim;
    public TextMeshProUGUI ComingSoon;

    void Start(){
        if(ComingSoon != null)
            ComingSoon.gameObject.SetActive(false);
    }

    public void playAnim(){
        anim.Play("LockAnim");

        if(ComingSoon != null)
            ComingSoon.gameObject.SetActive(true);
    }
}
