using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("DashInfo")]
    [SerializeField] private bool isDash;
    [SerializeField] private float dashSpeed;  // ��� �ӵ�
    [SerializeField] private float dashPower;   // ��� �Ŀ�
    [SerializeField] private float dashDuration = 0.5f; // ��� ���� �ð�; // ��� ���� �ð�
    [SerializeField] private float dashCooltime = 1f;   // ��� ��Ÿ��
    [SerializeField] private float dashDistance = 5f;  // ��� �Ÿ�
    [Header("SkillInfo")]
    [SerializeField] private bool usingSkillX;
    [SerializeField] private bool isAttack;
    [Header("WeaponHitBox")]
    [SerializeField]private Collider weaponHitBox;


    // Delegate ����
    private delegate void SkillDelegate();
    private SkillDelegate currentSkill;  // ���� ����� ��ų�� ���� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        isAttack = false;
        usingSkillX = false;
        isDash = false;

        // Delegate�� �ʱ� ��ų�� WhirlWind �޼��� �Ҵ�
        currentSkill = WhirlWind;   
    }
    private void Update()
    {
        // ���� �ִϸ��̼� ���°� "Idle"�̸� isAttack�� false�� ����
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAttack)
        {
            isAttack = false;  // ������ ������ Idle ������ �� �̵� �����ϰ� ����
            WeaponHitBoxTogle(weaponHitBox);
        }

        CalculateDirection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // ���� delegate�� ������ ��ų ����
            if (currentSkill != null)
            {
                Skill_X(currentSkill);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isDash == false)
        {
            StartCoroutine(Dodge());
        }
    }
    private void FixedUpdate()
    {
        RotatePlayer(); //ĳ���� ȸ��ó��
        MoveCharacter();    //ĳ���� �̵� ó��
    }

    protected override void CalculateDirection()
    {
        if (!usingSkillX && !isAttack)
            base.CalculateDirection();
    }
    protected override void MoveCharacter()
    {
        if (!usingSkillX && !isAttack && !isDash)
            base.MoveCharacter();
    }
    private void RotatePlayer() // ĳ���� ȸ��
    {
        if (dir != Vector3.zero && isDash == false)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }
    }
    //ȸ��
    private IEnumerator Dodge()
    {
        // ��� ���� Ȱ��ȭ
        isDash = true;
        animator.SetTrigger("Dodge");
        isAttack = false;
        //��ų ��� �� ȸ�� �� �ʱ�ȭ (��Ÿ���� �ʱ�ȭ X)
        if(usingSkillX == true)
        {
            usingSkillX = false;
        }

        // ��� ���۰� ��ǥ ��ġ ���
        Vector3 initialPosition = transform.position; // ��� ���� ��ġ
        Vector3 targetPosition = initialPosition + transform.forward * dashDistance; // ��� ���� ��ġ
        float dashTimeElapsed = 0f;

        // ��� ȿ�� (���� ���� ���� ��)

        while (dashTimeElapsed < dashDuration)
        {
            // ���� �ð��� �������� �ӵ� ����
            float remainingTime = dashDuration - dashTimeElapsed;

            // �̵� �ӵ� ��� (dashPower�� ������� ���� ����)
            float currentSpeed = dashPower * remainingTime / dashDuration;

            // �̵� ����
            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            transform.position += movement;

            dashTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // ��� ����
        isDash = false;

        // ���� ���̾� ����

        // ��� ��Ÿ�� ���� �߰� ��� �Ұ���
        yield return new WaitForSeconds(dashCooltime);
    }



    protected override void Attack()
    {
        isAttack = true;
        animator.SetTrigger("Attack");
        weaponHitBox.enabled = true;
    }
    public void WeaponHitBoxTogle(Collider weaponHitBox)
    {
        if(weaponHitBox.enabled == true)
        {
            weaponHitBox.enabled = false;
        }
    }


    private void Skill_X(SkillDelegate skillToExecute)  // �Ϲݽ�ų
    {
        if (skillToExecute != null && usingSkillX == false)
        {
            skillToExecute();  // �Ű������� ���޵� delegate ����
        }
    }
    private void Skill_C() //�ʻ��
    {

    }

    //SkillInterface�� ���� ó���� ��.
    private void WhirlWind()
    {
        usingSkillX = true;
        animator.SetTrigger("SKILL1");
    }
    public void isSkillActive()
    {
        usingSkillX = false;
    }
}