using UnityEngine;
using UnityEngine.UI;
public class EnemyDamage : MonoBehaviour
{   

    public int maxHp =50;
     int curHp = 50;           // 현재 cpfur


    public Image hpbar;

    private OneEneyAI oneEneyAI;
    private OneMoveAgent moveAgent;
    void Awake(){
        oneEneyAI = GetComponent<OneEneyAI>();
        moveAgent = GetComponent<OneMoveAgent>();
        hpbar.rectTransform.localScale = new Vector3(1f,1f,1f);
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag=="Bullet"){
        curHp-=25;
        hpbar.rectTransform.localScale = new Vector3(((float)curHp/(float)maxHp),1f,1f);
        if(curHp <=0){
            oneEneyAI.state = OneEneyAI.State.DIE;
        }
    }
    }

    private void GrenadeAttack(){
        curHp-=50;
        hpbar.rectTransform.localScale = new Vector3(((float)curHp/(float)maxHp),1f,1f);
        oneEneyAI.state = OneEneyAI.State.DIE;
    }
}
