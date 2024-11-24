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
        // ���� �ִϸ��̼� ���°� "Idle"�̸� isAttack�� false�� ����
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAttack)
        {
            isAttack = false;  // ������ ������ Idle ������ �� �̵� �����ϰ� ����
        }

        Move();

        // �ִϸ��̼� ������Ʈ
        animator.SetFloat("Speed", dir.magnitude);

        // ���� �Է� ó��
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
        // ȸ�� ó��
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }

        if(isSkill1Start == false || isAttack == false)
        {
            // �̵� ó��
            rb.MovePosition(transform.position + dir * speed * Time.deltaTime);
        }
        else 
            rb.velocity = Vector3.zero;
    }
    public void Move()
    {
        if (isSkill1Start == false && isAttack == false)
        {
            // �̵� ���� ���
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
