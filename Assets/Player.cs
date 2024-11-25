using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    [Header("WeaponTrail")]
    public GameObject WeaponTrail;
    public ParticleSystem[] AttackParticle;

    public float projectileSpeed;
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public Bullet bulletProjectile;


    // Delegate ����
    private delegate IEnumerator SkillDelegate();
    private SkillDelegate currentSkill;  // ���� ����� ��ų�� ���� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        isAttack = false;
        usingSkillX = false;
        isDash = false;
        WeaponTrail.SetActive(false);

        // Delegate�� �ʱ� ��ų�� WhirlWind �޼��� �Ҵ�
        currentSkill = WhirlWind;   
    }
    private void Update()
    {
        // ���� �ִϸ��̼� ���°� "ArchorIdle"�̸� isAttack�� false�� ����
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ArchorIdle") && isAttack)
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

        //�ǰ� �׽�Ʈ
        if(Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(TakeDamage(damage));
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
        WeaponTrail.SetActive(false);

        // ��ų ��� �� ȸ�� �� �ʱ�ȭ (��Ÿ���� �ʱ�ȭ X)
        if (usingSkillX == true)
        {
            usingSkillX = false;
        }

        // �Էµ� �̵� ���� ���
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // �Է��� ���� ��� ���� ����, ������ ���� �ٶ󺸴� ���� ����
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = targetRotation; // ĳ������ �ٶ󺸴� ���� ����
        }

        // ��� ����: �ٶ󺸴� ���� ����
        Vector3 dodgeDirection = transform.forward;

        // ��� ���� ��ġ�� ��ǥ ��ġ ���
        Vector3 initialPosition = transform.position; // ��� ���� ��ġ
        float dashTimeElapsed = 0f;

        // ��� ȿ�� (���� ���� ���� ��)
        while (dashTimeElapsed < dashDuration)
        {
            // ���� �ð��� �������� �ӵ� ����
            float remainingTime = dashDuration - dashTimeElapsed;

            // �̵� �ӵ� ��� (dashPower�� ������� ���� ����)
            float currentSpeed = dashPower * remainingTime / dashDuration;

            // �̵� ����
            Vector3 movement = dodgeDirection * currentSpeed * Time.deltaTime;
            transform.position += movement;

            dashTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // ��� ����
        isDash = false;

        // ��� ��Ÿ�� ���� �߰� ��� �Ұ���
        yield return new WaitForSeconds(dashCooltime);
    }


    public override IEnumerator TakeDamage(float damage)
    {
        isHit = true;   //�ǰ��߿� ����
        animator.SetTrigger("Hit");
        curHp -= damage;
        CameraShake.instance.ShakeCamera(1.5f, 0.5f);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        isHit = false;  //�ǰ� ���� ����
        Time.timeScale = 1f;
    }
    public override void Dead()
    {
        isDead = true;
    }

    public override void Attack()
    {
        // �Ѿ� ����
        Bullet bullet = Instantiate(bulletProjectile, firePoint.position, transform.rotation);
        // Bullet ��ũ��Ʈ�� �ӵ��� ������ ����
        bullet.Speed = projectileSpeed;
        bullet.Damage = damage; // �ʿ� �� �÷��̾��� ���ݷ��� �������� ����

        // ���� �ִϸ��̼� ����
        animator.SetTrigger("Attack");
    }
    public void WeaponHitBoxTogle(Collider weaponHitBox)
    {
        if(weaponHitBox.enabled == true)
        {
            WeaponTrail.SetActive(false);
            weaponHitBox.enabled = false;
        }
    }
    
    private void Skill_X(SkillDelegate skillToExecute)  // �Ϲݽ�ų
    {
        if (skillToExecute != null && usingSkillX == false)
        {
            StartCoroutine(skillToExecute());  // �Ű������� ���޵� delegate ����
        }
    }

    private void Skill_C() //�ʻ��
    {

    }
    public void isAttackFalse()
    {
        isAttack = true;
    }

    //SkillInterface�� ���� ó���� ��.
    private IEnumerator WhirlWind()
    {
        yield return null;
        usingSkillX = true;
        WeaponTrail.SetActive(true);
        animator.SetTrigger("SKILL1");
    }
    public void isSkillActive()
    {
        usingSkillX = false;
        WeaponTrail.SetActive(false);
    }
    public void ParticlePlay(int i)
    {
        // �÷��̾��� ��ġ�� �ٶ󺸴� �������� ��ƼŬ ����
        Vector3 particlePosition = transform.position + transform.forward * 1f; // 1f�� ��ƼŬ�� ������ �Ÿ��Դϴ�. �ʿ信 ���� ������ �� �ֽ��ϴ�.
                                                                                // ��ƼŬ�� �÷��̾� �տ� �����ǵ��� ��ġ�� ����
        ParticleSystem particle = Instantiate(AttackParticle[i], particlePosition, transform.rotation);
        // ��ƼŬ �ý����� StopAction�� Destroy�� �����մϴ�.
        var main = particle.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // ��ƼŬ�� ��ġ�� �����ϰ� ���
        particle.transform.position = particlePosition;
        particle.Play();
    }

}