using UnityEngine;

[CreateAssetMenu(menuName = "CameraShake/New Profile")]
public class CameraShakeProfile : ScriptableObject
{
    [Header("Impulse Source Setting")]
    public float ImpactTime = 0.2f;
    public float ImpactForce = 1f;
    public Vector3 DefaultVelocity = new Vector3(0f, -1f, 0f);
    public AnimationCurve ImpulseCurve;

    [Header("Impulse Listner Setting")]
    public float ListenerAmplitude = 1f;
    public float ListenerFrequency = 1f;
    public float ListenerDuration = 1f;
}