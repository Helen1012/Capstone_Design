using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoveAgent : MonoBehaviour
{
    //순찰 지점들을 저장하기 위한 List 타입 변수
    public List<Transform> wayPoints;

    //다음 순찰 지점의 배열 Index
    public int nextIdx = 0;

    //NavMeshAgent 컴포넌트를 저장할 변수
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        var group = GameObject.Find("WayPointGroup");
        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
        }
        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        if (agent.isPathStale) return;

        //다음 목적지를 wayPoints 배열에서 추출한 위치로 지정
        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false;
    }
    // Update is called once per frame
    void Update()
    {
        //NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부를 계산
        if(agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            //다음 목적지의 배열 첨자를 계산
            nextIdx = ++nextIdx % wayPoints.Count;
            //다음 목적지로 이동 명령을 수행
            MoveWayPoint();
        }
    }
}
