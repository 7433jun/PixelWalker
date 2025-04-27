using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Project.Enums;

public class BulletPool : MonoBehaviour
{
    public static BulletPool BP_Instance;
    private void Awake()
    {
        if(BP_Instance == null)
        {
            BP_Instance = this;
        }
    }

    [Header("Spawn Preset")]
    [SerializeField] Transform SpawnSpace;
    int AmountToPool = 10;

    List<GameObject> PrefabType_UnityChan_N = new List<GameObject>();
    List<GameObject> PrefabType_UnityChan_A = new List<GameObject>();
    List<GameObject> PrefabType_WindWisp = new List<GameObject>();
    List<GameObject> PrefabType_EarthWisp = new List<GameObject>();
    List<GameObject> PrefabType_WaterWisp = new List<GameObject>();

    [SerializeField] GameObject Bullet_UnityChan_N;
    [SerializeField] GameObject Bullet_UnityChan_A;
    [SerializeField] GameObject Bullet_WindWisp;
    [SerializeField] GameObject Bullet_EarthWisp;
    [SerializeField] GameObject Bullet_WaterWisp;

    [Header("Hit FX Preset")]
    List<GameObject> PrefabType_UnityChan_N_VFX = new List<GameObject>();
    List<GameObject> PrefabType_UnityChan_A_VFX = new List<GameObject>();
    List<GameObject> PrefabType_WindWisp_VFX = new List<GameObject>();
    List<GameObject> PrefabType_EarthWisp_VFX = new List<GameObject>();
    List<GameObject> PrefabType_WaterWisp_VFX = new List<GameObject>();

    [SerializeField] GameObject HitVFX_UnityChan_N;
    [SerializeField] GameObject HitVFX_UnityChan_A;
    [SerializeField] GameObject HitVFX_WindWisp;
    [SerializeField] GameObject HitVFX_EarthWisp;
    [SerializeField] GameObject HitVFX_WaterWisp;

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
        CreateObject(Bullet_UnityChan_N, PrefabType_UnityChan_N);
        CreateObject(Bullet_WindWisp, PrefabType_WindWisp);
        CreateObject(Bullet_EarthWisp, PrefabType_EarthWisp);
        CreateObject(Bullet_WaterWisp, PrefabType_WaterWisp);

        CreateObject(HitVFX_UnityChan_N, PrefabType_UnityChan_N_VFX);
        CreateObject(HitVFX_WindWisp, PrefabType_WindWisp_VFX);
        CreateObject(HitVFX_EarthWisp, PrefabType_EarthWisp_VFX);
        CreateObject(HitVFX_WaterWisp, PrefabType_WaterWisp_VFX);
    }

    GameObject AddToNewPrefab(GameObject Object, List<GameObject> ObjectList)
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

    public GameObject GetBulletTypeByBulletType(EProjectileType Type)
    {
        switch (Type)
        {
            case EProjectileType.UnityChan_Normal:

                return AddToNewPrefab(Bullet_UnityChan_N, PrefabType_UnityChan_N);

            case EProjectileType.UnityChan_Ability:

                return AddToNewPrefab(Bullet_UnityChan_A, PrefabType_UnityChan_A);

            case EProjectileType.WindWisp:

                return AddToNewPrefab(Bullet_WaterWisp, PrefabType_WaterWisp);

            case EProjectileType.EarthWisp:

                return AddToNewPrefab(Bullet_EarthWisp, PrefabType_EarthWisp);

            case EProjectileType.WaterWisp:

                return AddToNewPrefab(Bullet_WaterWisp, PrefabType_WaterWisp);
                
            default:
                Debug.LogError("Wrong Name! Searched Name: " + Type);

                break;
        }

        return null;
    }

    public GameObject GetBulletVFXTypeByBulletType(EProjectileType Type)
    {
        switch (Type)
        {
            case EProjectileType.UnityChan_Normal:

                return AddToNewPrefab(HitVFX_UnityChan_N, PrefabType_UnityChan_N_VFX);

            case EProjectileType.UnityChan_Ability:

                return AddToNewPrefab(HitVFX_UnityChan_A, PrefabType_UnityChan_A_VFX);

            case EProjectileType.WindWisp:

                return AddToNewPrefab(HitVFX_WindWisp, PrefabType_WindWisp_VFX);

            case EProjectileType.EarthWisp:

                return AddToNewPrefab(HitVFX_EarthWisp, PrefabType_EarthWisp_VFX);

            case EProjectileType.WaterWisp:

                return AddToNewPrefab(HitVFX_WaterWisp, PrefabType_WaterWisp_VFX);

            default:
                Debug.LogError("Bullet Hit VFX | Search Type Data Is Null");

                break;
        }

        return null;
    }
}
