using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//추상 클래스로 작성하지 않은 이유 Enemy 또한 상속을 받게 될 스크립트이기도 하고
//여러개의 캐릭터가 같은 키 세팅을 받는 게임이 아니기에 추상 클래스로 작성하지 않았습니다.
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Character : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] protected string CharacterName;
    [SerializeField] protected float maxHp;
    [SerializeField] protected float curHp;
    [SerializeField] protected float damage;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotSpeed;
    [SerializeField] protected bool isHit;
    [SerializeField] protected bool isDead;
    [SerializeField] protected Vector3 dir = Vector3.zero;


    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider col;
    [SerializeField] protected Animator animator;

    protected virtual void CalculateDirection()   //방향설정
    {
        // 이동 방향 계산
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        dir = new Vector3(horizontal, 0, vertical).normalized;
    }

    protected virtual void MoveCharacter() //캐릭터 이동
    {
        // 이동 처리
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
        animator.SetFloat("Speed", dir.magnitude);
    }
    protected abstract void Attack();  //일반공격
    protected abstract IEnumerator TakeDamage();   //피격
}
