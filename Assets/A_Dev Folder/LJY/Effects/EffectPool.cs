using Project.Enums;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Header("Spawn Preset")]
    [SerializeField] Transform SpawnSpace;
    int AmountToPool = 2;

    // Dodge Effects //
    List<GameObject> PrefabType_JustEvasionEffect = new List<GameObject>();
    List<GameObject> PrefabType_PerfectEvasionEffect = new List<GameObject>();
    // Recovery Effects //
    List<GameObject> PrefabType_HP_Recovery_Effect = new List<GameObject>();
    List<GameObject> PrefabType_HP_Regen_Effect = new List<GameObject>();
    List<GameObject> PrefabType_MP_RecoveryEffect = new List<GameObject>();
    // Buff Effects //
    List<GameObject> PrefabType_PowerUpEffect = new List<GameObject>();

    [Header("Dodge Effect")]
    [SerializeField] GameObject JustEvasionEffect;
    [SerializeField] GameObject PerfectEvasionEffect;

    [Header("Recovery Effect")]
    [SerializeField] GameObject HP_Recovery_Effect;
    [SerializeField] GameObject HP_Regen_Effect;
    [SerializeField] GameObject MP_RecoveryEffect;

    [Header("Buff Effect")]
    [SerializeField] GameObject PowerUp_Effect;

    void CreateObject(GameObject BulletType, List<GameObject> SpawnedBullet)
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            GameObject Obj = Instantiate(BulletType, SpawnSpace);

            Obj.SetActive(false);

            SpawnedBullet.Add(Obj);
        }
    }

    private void Start()
    {
        CreateObject(JustEvasionEffect, PrefabType_JustEvasionEffect);
        CreateObject(PerfectEvasionEffect, PrefabType_PerfectEvasionEffect);
        CreateObject(HP_Recovery_Effect, PrefabType_HP_Recovery_Effect);
        CreateObject(HP_Regen_Effect, PrefabType_HP_Regen_Effect);
        CreateObject(MP_RecoveryEffect, PrefabType_MP_RecoveryEffect);
        CreateObject(PowerUp_Effect, PrefabType_PowerUpEffect);
    }

    GameObject SearchToObject_If_NotEnogh_AddNew(GameObject Object, List<GameObject> ObjectList)
    {
        for (int i = 0; i < ObjectList.Count; i++)
        {
            if (!ObjectList[i].activeInHierarchy)
            {
                return ObjectList[i];
            }
        }

        // 풀에 사용 가능한 오브젝트가 없는 경우 새로 생성
        GameObject NewObject = Instantiate(Object);
        NewObject.SetActive(false); // 풀에 추가되기 전 비활성화
        ObjectList.Add(NewObject); // 풀에 등록
        return NewObject;
    }

    public GameObject GetEffectTypeByEffectType(EEffectType Type)
    {
        switch (Type)
        {
            case EEffectType.JustEvasion:

                return SearchToObject_If_NotEnogh_AddNew(JustEvasionEffect, PrefabType_JustEvasionEffect);

            case EEffectType.PerfectEvasion:

                return SearchToObject_If_NotEnogh_AddNew(PerfectEvasionEffect, PrefabType_PerfectEvasionEffect);

            case EEffectType.HP_Recovery:

                return SearchToObject_If_NotEnogh_AddNew(HP_Recovery_Effect, PrefabType_HP_Recovery_Effect);

            case EEffectType.HP_Regen:

                return SearchToObject_If_NotEnogh_AddNew(HP_Regen_Effect, PrefabType_HP_Regen_Effect);

            case EEffectType.MP_Recovery:

                return SearchToObject_If_NotEnogh_AddNew(MP_RecoveryEffect, PrefabType_MP_RecoveryEffect);

            case EEffectType.Buff_PowerUp:

                return SearchToObject_If_NotEnogh_AddNew(PowerUp_Effect, PrefabType_PowerUpEffect);

            default:
                Debug.LogError("Wrong Type! Searched Type: " + Type);

                break;
        }

        return null;
    }
}
