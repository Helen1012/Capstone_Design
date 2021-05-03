using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class OneMoveAgent : MonoBehaviour
{
     public List<Transform> wayPoints;
    private OneEneyAI oneEneyAI;
    private NavMeshAgent agent;

    private readonly float traceSpeed = 6.0f;
    private int idx;

    private float damping = 1.0f;

    private Transform enemyTr;
    // Start is called before the first frame update
    void Start()
    {   
        enemyTr = GetComponent<Transform>();
        oneEneyAI = GetComponent<OneEneyAI>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        var group = GameObject.Find("WayGroupList");

        if(group != null){
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
        }
       MoveWayPoint(); 
    }
    public float speed{
        get{return agent.velocity.magnitude;}
    }
    private void MoveWayPoint(){
        if(agent.isPathStale) return;
        idx = Random.Range(0,wayPoints.Count);
        agent.destination = wayPoints[idx].position;
        agent.speed = traceSpeed;
        oneEneyAI.state = OneEneyAI.State.MOVE;
        agent.isStopped = false;
    }
    public void Stop(){
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
        // 바로 정지하기 위해 속도 0
        agent.velocity = Vector3.zero;
      
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isStopped == false){
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }


            if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            oneEneyAI.state = OneEneyAI.State.ATTACK;
        }
    }
    public void DieStop(){
        agent.Stop();
    }
}
