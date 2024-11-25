using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : Character
{
    [Header("DashInfo")]
    [SerializeField] private bool isDash;
    [SerializeField] private float dashSpeed;  // 대시 속도
    [SerializeField] private float dashPower;   // 대시 파워
    [SerializeField] private float dashDuration = 0.5f; // 대시 지속 시간; // 대시 지속 시간
    [SerializeField] private float dashCooltime = 1f;   // 대시 쿨타임
    [SerializeField] private float dashDistance = 5f;  // 대시 거리
    [Header("SkillInfo")]
    [SerializeField] private bool usingSkillX;
    [SerializeField] private bool isAttack;
    [Header("WeaponHitBox")]
    [SerializeField]private Collider weaponHitBox;
    [Header("WeaponTrail")]
    public GameObject WeaponTrail;
    public ParticleSystem[] AttackParticle;

    public float projectileSpeed;
    public Transform firePoint; // 총알 발사 위치
    public Bullet bulletProjectile;


    // Delegate 선언
    private delegate IEnumerator SkillDelegate();
    private SkillDelegate currentSkill;  // 현재 사용할 스킬을 담을 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        isAttack = false;
        usingSkillX = false;
        isDash = false;
        WeaponTrail.SetActive(false);

        // Delegate에 초기 스킬인 WhirlWind 메서드 할당
        currentSkill = WhirlWind;   
    }
    private void Update()
    {
        // 현재 애니메이션 상태가 "ArchorIdle"이면 isAttack을 false로 설정
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ArchorIdle") && isAttack)
        {
            isAttack = false;  // 공격이 끝나고 Idle 상태일 때 이동 가능하게 설정
            WeaponHitBoxTogle(weaponHitBox);
        }

        CalculateDirection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // 현재 delegate에 설정된 스킬 실행
            if (currentSkill != null)
            {
                Skill_X(currentSkill);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isDash == false)
        {
            StartCoroutine(Dodge());
        }

        //피격 테스트
        if(Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(TakeDamage(damage));
        }
    }
    private void FixedUpdate()
    {
        RotatePlayer(); //캐릭터 회전처리
        MoveCharacter();    //캐릭터 이동 처리
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
    private void RotatePlayer() // 캐릭터 회전
    {
        if (dir != Vector3.zero && isDash == false)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }
    }
    //회피
    private IEnumerator Dodge()
    {
        // 대시 상태 활성화
        isDash = true;
        animator.SetTrigger("Dodge");
        isAttack = false;
        WeaponTrail.SetActive(false);

        // 스킬 사용 중 회피 시 초기화 (쿨타임은 초기화 X)
        if (usingSkillX == true)
        {
            usingSkillX = false;
        }

        // 입력된 이동 방향 계산
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // 입력이 있을 경우 방향 변경, 없으면 현재 바라보는 방향 유지
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = targetRotation; // 캐릭터의 바라보는 방향 변경
        }

        // 대시 방향: 바라보는 방향 기준
        Vector3 dodgeDirection = transform.forward;

        // 대시 시작 위치와 목표 위치 계산
        Vector3 initialPosition = transform.position; // 대시 시작 위치
        float dashTimeElapsed = 0f;

        // 대시 효과 (무적 상태 설정 등)
        while (dashTimeElapsed < dashDuration)
        {
            // 남은 시간을 기준으로 속도 조절
            float remainingTime = dashDuration - dashTimeElapsed;

            // 이동 속도 계산 (dashPower를 기반으로 점점 감소)
            float currentSpeed = dashPower * remainingTime / dashDuration;

            // 이동 벡터
            Vector3 movement = dodgeDirection * currentSpeed * Time.deltaTime;
            transform.position += movement;

            dashTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // 대시 종료
        isDash = false;

        // 대시 쿨타임 동안 추가 대시 불가능
        yield return new WaitForSeconds(dashCooltime);
    }


    public override IEnumerator TakeDamage(float damage)
    {
        isHit = true;   //피격중엔 무적
        animator.SetTrigger("Hit");
        curHp -= damage;
        CameraShake.instance.ShakeCamera(1.5f, 0.5f);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        isHit = false;  //피격 무적 해제
        Time.timeScale = 1f;
    }
    public override void Dead()
    {
        isDead = true;
    }

    public override void Attack()
    {
        // 총알 생성
        Bullet bullet = Instantiate(bulletProjectile, firePoint.position, transform.rotation);
        // Bullet 스크립트에 속도와 데미지 전달
        bullet.Speed = projectileSpeed;
        bullet.Damage = damage; // 필요 시 플레이어의 공격력을 데미지로 설정

        // 공격 애니메이션 실행
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
    
    private void Skill_X(SkillDelegate skillToExecute)  // 일반스킬
    {
        if (skillToExecute != null && usingSkillX == false)
        {
            StartCoroutine(skillToExecute());  // 매개변수로 전달된 delegate 실행
        }
    }

    private void Skill_C() //필살기
    {

    }
    public void isAttackFalse()
    {
        isAttack = true;
    }

    //SkillInterface를 만들어서 처리할 것.
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
        // 플레이어의 위치와 바라보는 방향으로 파티클 생성
        Vector3 particlePosition = transform.position + transform.forward * 1f; // 1f는 파티클이 생성될 거리입니다. 필요에 따라 조정할 수 있습니다.
                                                                                // 파티클이 플레이어 앞에 생성되도록 위치를 설정
        ParticleSystem particle = Instantiate(AttackParticle[i], particlePosition, transform.rotation);
        // 파티클 시스템의 StopAction을 Destroy로 설정합니다.
        var main = particle.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // 파티클의 위치를 설정하고 재생
        particle.transform.position = particlePosition;
        particle.Play();
    }

}