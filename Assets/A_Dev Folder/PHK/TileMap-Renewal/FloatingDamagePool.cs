using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingDamagePool : MonoBehaviour
{
    public static FloatingDamagePool Instance { get; private set; }

    [SerializeField] GameObject floatingDamagePrefab;
    [SerializeField] int initPoolSize = 10;
    [SerializeField] int maxPoolSize = 50;

    private readonly Queue<GameObject> prefabPool = new Queue<GameObject>();

    //public enum DamageType
    //{
    //    Default,
    //    Critical,
    //    Heal,
    //}

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < initPoolSize; i++)
        {
            AddObjectToPool();
        }
    }

    private GameObject AddObjectToPool()
    {
        if (prefabPool.Count >= maxPoolSize) return null;

        GameObject obj = Instantiate(floatingDamagePrefab, transform);
        obj.SetActive(false);
        prefabPool.Enqueue(obj);
        return obj;
    }

    public static void CallFloatingDamage(Transform target, float damage)
    {
        if (Instance == null) return;

        GameObject obj;

        if (Instance.prefabPool.Count > 0)
        {
            obj = Instance.prefabPool.Dequeue();
        }
        else
        {
            obj = Instance.AddObjectToPool();
            if (obj == null) return;
        }

        obj.transform.SetAsLastSibling();
        obj.SetActive(true);

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);

        Vector2 canvasPoint = new Vector2(screenPoint.x - (Screen.width / 2), screenPoint.y - (Screen.height / 2));

        obj.GetComponent<FloatingDamage>().SetUI(canvasPoint, Mathf.RoundToInt(damage));
    }

    public static void CallFloatingDamage(Project.Enums.DamageType damageType, Transform target, float damage)
    {
        if (Instance == null) return;

        GameObject obj;

        if (Instance.prefabPool.Count > 0)
        {
            obj = Instance.prefabPool.Dequeue();
        }
        else
        {
            obj = Instance.AddObjectToPool();
            if (obj == null) return;
        }

        obj.transform.SetAsLastSibling();
        obj.SetActive(true);

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);

        Vector2 canvasPoint = new Vector2(screenPoint.x - (Screen.width / 2), screenPoint.y - (Screen.height / 2));

        obj.GetComponent<FloatingDamage>().SetUI(damageType, canvasPoint, Mathf.RoundToInt(damage));
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        prefabPool.Enqueue(obj);
    }
}