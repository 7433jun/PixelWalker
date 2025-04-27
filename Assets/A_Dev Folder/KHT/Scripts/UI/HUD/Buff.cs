using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    public static Buff Instance { get; private set; }

    private static List<int> buffList;
    private static List<GameObject> buffObjects;

    private static BuffInfo buffInfo;
    private static GameObject buffPrefab;
    private static Transform thisTransform;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        buffList = new List<int>();
        buffObjects = new List<GameObject>();

        buffInfo = Resources.Load<BuffInfo>("KHT/ScriptableObjects/Buff Info");
        buffPrefab = Resources.Load<GameObject>("KHT/Prefabs/BuffPrefab");
        thisTransform = transform;
    }

    public static void OnUI(int buffIndex)
    {
        if (!CheckReosurces())
            return;

        if (buffList.Contains(buffIndex))
        {
            Debug.Log($"{buffIndex} buffIndex is already On");
            return;
        }

        buffList.Add(buffIndex);
        GameObject buffObject = Instantiate(buffPrefab, thisTransform);
        buffObject.GetComponent<Image>().sprite = buffInfo.BuffSpriteList[buffIndex];
        buffObjects.Add(buffObject);
        Set();
    }

    public static void OffUI(int buffIndex)
    {
        if (!CheckReosurces())
            return;

        int index = buffList.IndexOf(buffIndex);

        if (index == -1)
        {
            Debug.Log($"{buffIndex} buffIndex is already Off");
            return;
        }

        buffList.Remove(buffIndex);
        GameObject buffObject = buffObjects[index];
        buffObjects.RemoveAt(index);
        Destroy(buffObject);
        Set();
    }

    private static void Set()
    {
        for (int i = 0; i < buffObjects.Count; i++)
        {
            RectTransform rectTransform = buffObjects[i].GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.anchoredPosition = new Vector2(50 * i, 0);
            }
        }
    }

    private static bool CheckReosurces()
    {
        if (!buffInfo)
        {
            Debug.LogError("KHT/ScriptableObjects/Buff Info is not exist");
            return false;
        }
        else if (!buffPrefab)
        {
            Debug.LogError("KHT/Prefabs/BuffPrefab is not exist");
            return false;
        }

        return true;
    }
}
