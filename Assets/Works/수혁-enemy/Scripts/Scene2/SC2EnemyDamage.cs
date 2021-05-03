using UnityEngine;
public class SC2EnemyDamage : MonoBehaviour
{   

    private const string bulletTag = "Bullet";
    private float hp = 100.0f;

    //private float initHP = 100.0f;
    private GameObject bloodEffect;

    // Start is called before the first frame update
    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("Big");
    }


    private void OnCollisionEnter(Collision coll){
        if(coll.collider.tag  == bulletTag){
            ShowBloodEffect(coll);
            Destroy(coll.gameObject);
            this.SendMessage("UpdateAfterReceiveAttack");
            hp-= 34f;
            if(hp <=0.0f){
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
            }
        }
        
    }
    
    private void GrenadeAttack(){
        hp-=100;
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
        this.SendMessage("UpdateAfterReceiveGranadeAttack");
        
    }
    
    private void ShowBloodEffect(Collision coll){
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }

}
