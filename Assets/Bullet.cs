using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public float Lifetime = 5f; // �Ѿ��� ���� �ð� (��)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �Ѿ��� �������� �̵�
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� ������ Ȯ�� 
        // ������ ������ ���� (Enemy ��ũ��Ʈ �ʿ�)

        // �浹 �� �Ѿ� �ı�
    }
}
