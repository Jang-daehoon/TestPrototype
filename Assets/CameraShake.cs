using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public CinemachineVirtualCamera virtualCamera;
    private float originalCamFovValue;
    public float hitCamFovValue;
    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private float shakeDuration = 0f; // ��鸲 ���� �ð�

    private void Awake()
    {
        originalCamFovValue = virtualCamera.m_Lens.FieldOfView;
        instance = this;
        // Perlin Noise ������Ʈ ��������
        perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        StartCoroutine(StopShake());
    }

    public void ShakeCamera(float intensity, float duration)
    {
        // ��鸲 ����
        perlinNoise.m_AmplitudeGain = intensity;
        perlinNoise.m_FrequencyGain = intensity;
        shakeDuration = duration;

        // ��鸲 ȿ�� ���� �ڷ�ƾ ����
        StartCoroutine(StopShake());
    }

    private IEnumerator StopShake()
    {
        while (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        // ��鸲 ȿ�� ����
        perlinNoise.m_AmplitudeGain = 0f;
        perlinNoise.m_FrequencyGain = 0f;
    }
    //ī�޶� Ȯ��
    public void ChangeFOV(float newFOV)
    {
        // virtualCamera�� m_Lens �Ӽ��� �����Ͽ� FOV ���� ����
        virtualCamera.m_Lens.FieldOfView = newFOV;
    }
}
