using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    static public StatusEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 이미 싱글톤 인스턴스가 존재하면 자신을 파괴
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬 전환 시 삭제되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    [Header("Status Effector")]
    public BleedingEffect BleedingMasterEffector;
    public PoisoningEffect PoisoningMasterEffector;
}
