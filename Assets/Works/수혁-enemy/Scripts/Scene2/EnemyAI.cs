using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State{
        PATROL,
        TRACE,
        ATTACK,
        MELLE_ATTACK,
        MELLE_TRACE,
        DIE
    }

    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    // Animator 컴포넌트를 저장할 변수
    private Animator animator;


    private float attackDist = 4f;
    public float traceDist = 10.0f;

    
    private GameObject target;
    private SC2PlayerDamage playerDamage;
    public bool isDie = false;

    private WaitForSeconds ws;

    // 이동을 제어하는 MoveAgent클래스 저장할 변수
    private MoveAgent moveAgent;


    private EnemyFire enemyFire;
    private EnemyMeleeAttack enemyMeleeAttack;  // 근접공격스크립트

    // 애니메이터 컨트롤러에 정의한 파라미터 해시값 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    
    private readonly int hashOffset = Animator.StringToHash("Offset");
    
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    private SC2HpBar hp;    
    
    private readonly int hashMeleeAttack = Animator.StringToHash("MeleeAttack");
    private readonly int hashMeleeAttackIdx = Animator.StringToHash("MeleeAttackIdx");
    private readonly int hashDance = Animator.StringToHash("isDance");

    private float DieDelayTime = 10.0f;

    private void Awake(){
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null) playerTr = player.GetComponent<Transform>();

        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();
        ws = new WaitForSeconds(0.3f);
        hp = GetComponent<SC2HpBar>();
        target = GameObject.Find("Handgun_01_FPSController");
        playerDamage = target.GetComponent<SC2PlayerDamage>();
        enemyMeleeAttack = GetComponent<EnemyMeleeAttack>();
        animator.SetFloat(hashOffset, Random.Range(0.0f,1.0f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f,1.2f));
    }

    private void OnEnable(){
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }
    IEnumerator Action(){
        while(!isDie){
            yield return ws;
            // 상태에 따라 분기처리
            switch(state){
                case State.PATROL:
                    // 총알발사정지
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    if(enemyFire.isFire == false) enemyFire.isFire = true;
                    break;
                case State.MELLE_ATTACK:
                    
                    moveAgent.Stop();
                    animator.SetBool(hashMove,false);
                    if(enemyMeleeAttack.isMeleeAttack ==false) enemyMeleeAttack.isMeleeAttack = true;
                    break;

                case State.MELLE_TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;

                   
                case State.DIE:
                    this.gameObject.tag = "Untagged";
                    isDie = true;
                    enemyFire.isFire = false;
                    enemyMeleeAttack.isMeleeAttack = false;                    
                    moveAgent.Stop();
                    int ran = Random.Range(0,3);
                    animator.SetInteger(hashDieIdx,ran);
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject,DieDelayTime);
                    break;
            }
        }
    }

    IEnumerator CheckState(){
        while(!isDie){
            if(state == State.DIE) yield break;

            if(state == State.MELLE_ATTACK) yield break;    

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);


            if(state == State.MELLE_TRACE && dist <=0.7f){
                state = State.MELLE_ATTACK;
            }
            else if (dist <=attackDist){
                if(hp.curHp <= 33){
                    state = State.MELLE_TRACE;
                }
                else{
                    state = State.ATTACK;
                }
            }
            else if(dist <=traceDist){
                state = State.TRACE;
            }
            else{
                state = State.PATROL;
            }
            yield return ws;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }


    public void OnPlayerDie(){
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines();

        animator.SetTrigger(hashDance);
    }
}