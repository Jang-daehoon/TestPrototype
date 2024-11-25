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
    // 씬을 이동하는 메서드
    public void MoveToScene(string sceneName)
    {
        // 씬 이름을 사용하여 씬 비동기 전환
        SceneManager.LoadSceneAsync(sceneName);
    }
}
