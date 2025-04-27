using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; // 실제 인스턴스를 저장

    public AudioSource BGMPlayer;
    public AudioSource SystemPlayer;
    public AudioSource EffectPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region Sound Value
    [SerializeField] float MasterVolume = 1;

    [SerializeField] float BgmVolume = 1;
    [SerializeField] float VoiceVolume = .1f;
    [SerializeField] float EffectVolume = 1; // SFX
    [SerializeField] float SystemVolume = 1; // UI 클릭음 등

    public float GetBgmVolume()
    {
        return Mathf.Clamp(MasterVolume * BgmVolume, 0, 1);
    }

    public float GetVoiceVolume()
    {
        return Mathf.Clamp(MasterVolume * VoiceVolume, 0, 1);
    }

    public float GetEffectVolume()
    {
        return Mathf.Clamp(MasterVolume * EffectVolume, 0, 1);
    }

    public float GetSystemVolume()
    {
        return Mathf.Clamp(MasterVolume * SystemVolume, 0, 1);
    }
    #endregion

}
