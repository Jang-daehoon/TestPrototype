using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [SerializeField] private State enemyState;
    NavMeshAgent agent;
    public Transform TargetPos;
    [SerializeField] private float chaseRange = 1f; //target과의 거리 (공격범위)
    [SerializeField] private float attackInterval = 1f; //공격쿨타임
    private float distanceToTarget = Mathf.Infinity;

    private bool isProvoked = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        isProvoked = true;
    }
    private void Start()
    {
        agent.stoppingDistance = chaseRange;
    }
    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(TargetPos.position, transform.position);

        if(isProvoked == true)
        {
            EngageTarget();
        }
        else if(distanceToTarget <= chaseRange)
        {
            isProvoked = true;
        }
    }
private void EngageTarget()
    {
        //target과의 거리가 제동 거리보다 같거나 크다면 계속 추적
        if (distanceToTarget >= agent.stoppingDistance)
            ChaseTarget();
        //현재 target과의 거리가 제동거리보다 같거나 작으면 공격
        if (distanceToTarget <= agent.stoppingDistance)
            AttackTarget();
    }

    private void AttackTarget()
    {
        Attack();
    }

    private void ChaseTarget()
    {
        agent.SetDestination(TargetPos.position);
    }
    public override void Attack()
    {
        Debug.Log("공격");
    }
    public override IEnumerator TakeDamage(float damage)
    {
        yield return null;
        curHp -= damage;
        Dead();
    }
    public override void Dead()
    {
        isDead = true;

    }
    private enum State
    {
        IDLE, CHASE, ATTACK
    }
}
