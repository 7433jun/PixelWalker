using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    static public StatusEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // �̹� �̱��� �ν��Ͻ��� �����ϸ� �ڽ��� �ı�
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // �� ��ȯ �� �������� �ʵ��� ����
        DontDestroyOnLoad(gameObject);
    }

    [Header("Status Effector")]
    public BleedingEffect BleedingMasterEffector;
    public PoisoningEffect PoisoningMasterEffector;
}
