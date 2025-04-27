using Project.Enums;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectPool : MonoBehaviour
{
    public static FallingObjectPool Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Header("Spawn Preset")]
    [SerializeField] Transform SpawnSpace;
    int AmountToPool = 3;

    List<GameObject> PrefabType_Rock = new List<GameObject>();

    [SerializeField] GameObject Rock;

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
        CreateObject(Rock, PrefabType_Rock);
    }

    GameObject AddToNewPrefab(GameObject Object, List<GameObject> ObjectList)
    {
        // Ǯ�� ��� ������ ������Ʈ�� ���� ��� ���� ����
        GameObject NewObject = Instantiate(Object);
        NewObject.SetActive(false); // Ǯ�� �߰��Ǳ� �� ��Ȱ��ȭ
        ObjectList.Add(NewObject); // Ǯ�� ���
        return NewObject;
    }


    public GameObject GetFallingObjectByObjectType(EFallingObjectType Type)
    {
        switch (Type)
        {
            case EFallingObjectType.Rock:
                for (int i = 0; i < PrefabType_Rock.Count; i++)
                {
                    if (!PrefabType_Rock[i].activeInHierarchy)
                    {
                        return PrefabType_Rock[i];
                    }
                }

                return AddToNewPrefab(Rock, PrefabType_Rock);

            default:
                Debug.LogWarning("Please Update to FallingObjectPool");
                break;
        }

        return null;
    }
}
