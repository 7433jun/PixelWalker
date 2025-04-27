using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] float GlobaslShakeForce = 1f;
    [SerializeField] CinemachineImpulseListener ImpulseListener;

    CinemachineImpulseDefinition ImpulseDefinition;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GameObject ListenerObject = GameObject.FindGameObjectWithTag("Cinemachine");
        ImpulseListener = ListenerObject.GetComponent<CinemachineImpulseListener>();
    }

    public void GenericCameraShake(CinemachineImpulseSource ImpulseSource)
    {
        ImpulseSource.GenerateImpulseWithForce(GlobaslShakeForce);
    }

    #region Just 1 Shaking
    public void CameraShakeFromProfile(CameraShakeProfile ProfileData, CinemachineImpulseSource ImpulseSource)
    {
        if(!ProfileData)
        {
            Debug.LogWarning("Is Not Set Character Shake Profile");
            return;
        }

        if(!ImpulseSource)
        {
            Debug.LogWarning("Is Not Set Character Component: Impulse Source");
            return;
        }

        // Apply Setting
        SetupCameraShakeSetting(ProfileData, ImpulseSource);

        // Camera Shake
        ImpulseSource.GenerateImpulseWithForce(ProfileData.ImpactForce);
    }

    public void SetupCameraShakeSetting(CameraShakeProfile ProfileData, CinemachineImpulseSource ImpulseSource)
    {
        ImpulseDefinition = ImpulseSource.ImpulseDefinition;

        // Impulse Source Setting
        ImpulseDefinition.ImpulseDuration = ProfileData.ImpactTime;
        ImpulseSource.DefaultVelocity = ProfileData.DefaultVelocity;
        ImpulseDefinition.CustomImpulseShape = ProfileData.ImpulseCurve;

        // Impulse Listener Setting
        ImpulseListener.ReactionSettings.AmplitudeGain = ProfileData.ListenerAmplitude;
        ImpulseListener.ReactionSettings.FrequencyGain = ProfileData.ListenerFrequency;
        ImpulseListener.ReactionSettings.Duration = ProfileData.ListenerDuration;
    }
    #endregion

    #region Shaking to foreach
    public void StartCameraShake(CameraShakeProfile ProfileData, CinemachineImpulseSource ImpulseSource, float ImpulseDuration, float ShakeDuration, float ImpactForce, float Magnitude)
    {
        SetupCameraShakeSetting(ProfileData, ImpulseSource); // 초기 설정
        ImpulseDefinition.ImpulseDuration = ImpulseDuration;

        StartCoroutine(CameraShake(ShakeDuration, ImpulseSource, ImpactForce, Magnitude));
    }

    IEnumerator CameraShake(float Duration, CinemachineImpulseSource ImpulseSource, float ImpactForce, float Magnitude)
    {
        float OriginDuration = Duration;
        float InImpactForce;
        // Duration, Magnitude
        while(Duration > 0)
        {
            InImpactForce = Random.Range(0.5f, ImpactForce);

            if (Duration < OriginDuration / 10)
            {
                // 지속시간이 거의 다 끝나간다면
                InImpactForce *= .5f; // 위력을 빠른 속도로 감소시킴
            }

            ImpulseSource.DefaultVelocity = SetupCameraShakeSettingToRandomVelocityFrom2D(Magnitude);

            ImpulseSource.GenerateImpulseWithForce(InImpactForce);
            Duration -= Time.deltaTime;
            yield return null;
        }
    }

    Vector2 SetupCameraShakeSettingToRandomVelocityFrom2D(float Magnitude)
    {
        // 진동 방향의 수정
        float x = Random.Range(-1f, 1f) * Magnitude;
        float y = Random.Range(-1f, 1f) * Magnitude;

        return new Vector2(x, y);
    }

    #endregion
}
