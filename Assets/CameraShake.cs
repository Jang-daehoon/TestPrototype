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
    private float shakeDuration = 0f; // 흔들림 지속 시간

    private void Awake()
    {
        originalCamFovValue = virtualCamera.m_Lens.FieldOfView;
        instance = this;
        // Perlin Noise 컴포넌트 가져오기
        perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        StartCoroutine(StopShake());
    }

    public void ShakeCamera(float intensity, float duration)
    {
        // 흔들림 설정
        perlinNoise.m_AmplitudeGain = intensity;
        perlinNoise.m_FrequencyGain = intensity;
        shakeDuration = duration;

        // 흔들림 효과 중지 코루틴 실행
        StartCoroutine(StopShake());
    }

    private IEnumerator StopShake()
    {
        while (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;
            yield return null;
        }

        // 흔들림 효과 종료
        perlinNoise.m_AmplitudeGain = 0f;
        perlinNoise.m_FrequencyGain = 0f;
    }
    //카메라 확대
    public void ChangeFOV(float newFOV)
    {
        // virtualCamera의 m_Lens 속성에 접근하여 FOV 값을 변경
        virtualCamera.m_Lens.FieldOfView = newFOV;
    }
}
