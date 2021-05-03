using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneEneyAI : MonoBehaviour
{   
        
        public enum State{
        MOVE,
        ATTACK,
        DIE
    }
    private Transform playerTr;
    private Transform enemyTr;

    public bool isDie = false;
    
    private WaitForSeconds ws;
    public State state = State.MOVE;
    private Animator animator;
    

    private OneEnmeyFire enmeyFire;
    private readonly int hashFire = Animator.StringToHash("isFire");
     private float DieDelayTime = 10.0f;
    private readonly int hashMove = Animator.StringToHash("isRun");
    private readonly int hashSpeed = Animator.StringToHash("walkSpeed");

    private readonly int hashDieIdx = Animator.StringToHash("SC1DieIdx");

    private readonly int hashDie = Animator.StringToHash("SC1Die");
    private readonly int hashDance = Animator.StringToHash("isDance");
    private OneMoveAgent moveAgent;
    private readonly float damping = 10.0f;
    private void Awake(){
        
        var player = GameObject.Find("TmpPlayer");

        if(player != null) playerTr = player.GetComponent<Transform>();

        enemyTr = GetComponent<Transform>();
        ws = new WaitForSeconds(0.3f);
        animator = GetComponent<Animator>();
        moveAgent = GetComponent<OneMoveAgent>();
        enmeyFire = GetComponent<OneEnmeyFire>();
    }

    private void OnEnable(){
        StartCoroutine(Action());
    }

    IEnumerator Action(){
        while(!isDie){
            yield return ws;
            // 상태에 따라 분기처리
            switch(state){
                case State.MOVE:
                    // 총알발사정지
                    enmeyFire.isFire = false;
                    //moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    transform.LookAt(playerTr);
                    animator.SetBool(hashMove,false);
                    if(enmeyFire.isFire == false) enmeyFire.isFire = true;
                    break;

                case State.DIE:
                    this.gameObject.tag = "Untagged";
                    moveAgent.Stop();
                    enmeyFire.isFire = false;
                    isDie = true;
                    int ran = Random.Range(1,5);
                    animator.SetInteger(hashDieIdx,ran);
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject,DieDelayTime);
                    break;
                
  
            }
        }
    }

    public void OnPlayerDie(){
        moveAgent.Stop();
        enmeyFire.isFire = false;
        StopAllCoroutines();

        animator.SetTrigger(hashDance);
    }
    void Update(){

        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}
