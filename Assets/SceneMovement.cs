using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMovement : MonoBehaviour
{
    public string SceneName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MoveToScene(SceneName);
        }
    }
    // ���� �̵��ϴ� �޼���
    public void MoveToScene(string sceneName)
    {
        // �� �̸��� ����Ͽ� �� �񵿱� ��ȯ
        SceneManager.LoadSceneAsync(sceneName);
    }
}
