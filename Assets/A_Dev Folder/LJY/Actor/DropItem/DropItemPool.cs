using System.Collections.Generic;
using UnityEngine;
using Project.Enums;

public class DropItemPool : MonoBehaviour
{
    public static DropItemPool Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Header("Spawn Preset")]
    [SerializeField] Transform SpawnSpace;
    int AmountToPool = 5;

    List<GameObject> PrefabType_HP_Potion = new List<GameObject>();
    List<GameObject> PrefabType_MP_Potion = new List<GameObject>();
    List<GameObject> PrefabType_Gold = new List<GameObject>();

    [SerializeField] GameObject HP_Potion;
    [SerializeField] GameObject MP_Potion;
    [SerializeField] GameObject Gold;

    void CreateObject(GameObject ItemType, List<GameObject> SpawnedItem)
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            GameObject Obj = Instantiate(ItemType, SpawnSpace);

            Obj.SetActive(false);

            SpawnedItem.Add(Obj);
        }
    }

    private void Start()
    {
        CreateObject(HP_Potion, PrefabType_HP_Potion);
        CreateObject(MP_Potion, PrefabType_MP_Potion);
        CreateObject(Gold, PrefabType_Gold);
    }

    GameObject AddToNewPrefab(GameObject Object, List<GameObject> ObjectList)
    {
        // 풀에 사용 가능한 오브젝트가 없는 경우 새로 생성
        GameObject NewObject = Instantiate(Object);
        NewObject.SetActive(false); // 풀에 추가되기 전 비활성화
        ObjectList.Add(NewObject); // 풀에 등록
        return NewObject;
    }


    public GameObject GetItemByItemType(EItemType Type)
    {
        switch (Type)
        {
            case EItemType.HP_Potion:

                for (int i = 0; i < PrefabType_HP_Potion.Count; i++)
                {
                    if (!PrefabType_HP_Potion[i].activeInHierarchy)
                    {
                        return PrefabType_HP_Potion[i];
                    }
                }

                return AddToNewPrefab(HP_Potion, PrefabType_HP_Potion);

            case EItemType.MP_Potion:

                for (int i = 0; i < PrefabType_MP_Potion.Count; i++)
                {
                    if (!PrefabType_MP_Potion[i].activeInHierarchy)
                    {
                        return PrefabType_MP_Potion[i];
                    }
                }

                return AddToNewPrefab(MP_Potion, PrefabType_MP_Potion);

            case EItemType.Gold:

                for (int i = 0; i < PrefabType_Gold.Count; i++)
                {
                    if (!PrefabType_Gold[i].activeInHierarchy)
                    {
                        return PrefabType_Gold[i];
                    }
                }

                return AddToNewPrefab(Gold, PrefabType_Gold);

            default:
                Debug.LogError("Wrong ItemType! Searched");

                break;
        }

        return null;
    }
}
