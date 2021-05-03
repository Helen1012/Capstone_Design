using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SC2HpBar : MonoBehaviour
{   
     public float maxHp =100f;
     public float curHp = 100f;           // 인스펙터에서 설정잘하기


    public Image hpbar;

    void Awake(){
        hpbar.rectTransform.localScale = new Vector3(1f,1f,1f);
    
    }

    
    public void UpdateAfterReceiveAttack(){
        curHp-=34.0f;

        if(curHp <=0){

            GameObject hp = gameObject.transform.Find("HPBarPos").gameObject;
            hp.SetActive(false);
 
        }
        hpbar.rectTransform.localScale = new Vector3((float)curHp/(float)maxHp,1f,1f);
    }

        public void UpdateAfterReceiveGranadeAttack(){
            curHp-=100.0f;


            GameObject hp = gameObject.transform.Find("HPBarPos").gameObject;
            hp.SetActive(false);

            hpbar.rectTransform.localScale = new Vector3((float)curHp/(float)maxHp,1f,1f);
    }
 
}
