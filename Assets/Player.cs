using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Delegate 선언
    private delegate void SkillDelegate();
    private SkillDelegate currentSkill;  // 현재 사용할 스킬을 담을 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        isAttack = false;
        usingSkillX = false;
        isDash = false;

        // Delegate에 초기 스킬인 WhirlWind 메서드 할당
        currentSkill = WhirlWind;   
    }
    private void Update()
    {
        // 현재 애니메이션 상태가 "Idle"이면 isAttack을 false로 설정
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAttack)
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
        //스킬 사용 중 회피 시 초기화 (쿨타임은 초기화 X)
        if(usingSkillX == true)
        {
            usingSkillX = false;
        }

        // 대시 시작과 목표 위치 계산
        Vector3 initialPosition = transform.position; // 대시 시작 위치
        Vector3 targetPosition = initialPosition + transform.forward * dashDistance; // 대시 종료 위치
        float dashTimeElapsed = 0f;

        // 대시 효과 (무적 상태 설정 등)

        while (dashTimeElapsed < dashDuration)
        {
            // 남은 시간을 기준으로 속도 조절
            float remainingTime = dashDuration - dashTimeElapsed;

            // 이동 속도 계산 (dashPower를 기반으로 점점 감소)
            float currentSpeed = dashPower * remainingTime / dashDuration;

            // 이동 벡터
            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            transform.position += movement;

            dashTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // 대시 종료
        isDash = false;

        // 무적 레이어 복구

        // 대시 쿨타임 동안 추가 대시 불가능
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


    private void Skill_X(SkillDelegate skillToExecute)  // 일반스킬
    {
        if (skillToExecute != null && usingSkillX == false)
        {
            skillToExecute();  // 매개변수로 전달된 delegate 실행
        }
    }
    private void Skill_C() //필살기
    {

    }

    //SkillInterface를 만들어서 처리할 것.
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