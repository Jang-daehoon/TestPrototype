using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;

    public float speed = 10f;
    public float dash = 5f;
    public float rotSpeed = 3f;
    public int hashAttackCnt = Animator.StringToHash("AttackCount");
    private Vector3 dir = Vector3.zero;
    public bool isSkill1Start;
    public bool isAttack;
    [SerializeField] private bool isGround = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
    }

    void Start()
    {
        isAttack = false;
        isSkill1Start = false;
    }

    void Update()
    {
        // 현재 애니메이션 상태가 "Idle"이면 isAttack을 false로 설정
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAttack)
        {
            isAttack = false;  // 공격이 끝나고 Idle 상태일 때 이동 가능하게 설정
        }

        Move();

        // 애니메이션 업데이트
        animator.SetFloat("Speed", dir.magnitude);

        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isAttack = true;
            animator.SetTrigger("Attack");
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(WhirlWind());
        }
    }

    private void FixedUpdate()
    {
        // 회전 처리
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }

        if(isSkill1Start == false || isAttack == false)
        {
            // 이동 처리
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
        }
        else 
            rb.velocity = Vector3.zero;
    }
    public void Move()
    {
        if (isSkill1Start == false && isAttack == false)
        {
            // 이동 방향 계산
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            dir = new Vector3(horizontal, 0, vertical).normalized;
        }

    }
    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCnt);
        set => animator.SetInteger(hashAttackCnt, value);
    }
    private IEnumerator WhirlWind()
    {
        isSkill1Start = true;
        animator.SetTrigger("SKILL1");
        yield return new WaitForSeconds(2f);
        isSkill1Start = false;
    }
    public void AtkTureTiming()
    {
        isAttack = true;
    }
}
