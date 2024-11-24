using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//�߻� Ŭ������ �ۼ����� ���� ���� Enemy ���� ����� �ް� �� ��ũ��Ʈ�̱⵵ �ϰ�
//�������� ĳ���Ͱ� ���� Ű ������ �޴� ������ �ƴϱ⿡ �߻� Ŭ������ �ۼ����� �ʾҽ��ϴ�.
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

    protected virtual void CalculateDirection()   //���⼳��
    {
        // �̵� ���� ���
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        dir = new Vector3(horizontal, 0, vertical).normalized;
    }

    protected virtual void MoveCharacter() //ĳ���� �̵�
    {
        // �̵� ó��
        rb.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
        animator.SetFloat("Speed", dir.magnitude);
    }
    protected abstract void Attack();  //�Ϲݰ���
    protected abstract IEnumerator TakeDamage();   //�ǰ�
}
