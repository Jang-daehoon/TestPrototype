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
    [SerializeField] private float chaseRange = 1f; //target���� �Ÿ� (���ݹ���)
    [SerializeField] private float attackInterval = 1f; //������Ÿ��
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
        //target���� �Ÿ��� ���� �Ÿ����� ���ų� ũ�ٸ� ��� ����
        if (distanceToTarget >= agent.stoppingDistance)
            ChaseTarget();
        //���� target���� �Ÿ��� �����Ÿ����� ���ų� ������ ����
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
        Debug.Log("����");
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
