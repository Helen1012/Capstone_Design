using UnityEngine;

public class LockClicked : MonoBehaviour
{
    public void click(){
        if(FindObjectOfType<GameManager>().isPlayerWininFirst)
            GetComponent<Restart>().playShow();
        else
            GetComponentInChildren<PlayAnimInLock>().playAnim();
    }
}
