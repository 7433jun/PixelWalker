using System.Collections;
using TMPro;
using UnityEngine;
using Project.Enums;

public class FloatingDamage : MonoBehaviour
{
    public float appearRadius;
    public float speed;
    public float floatingTime;



    private RectTransform rectTransform;
    private TextMeshProUGUI damageText;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        damageText = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        StartCoroutine(FloatingEffect());
    }

    public void SetUI(Vector2 position, int damage)
    {
        rectTransform.anchoredPosition = position + Random.insideUnitCircle * appearRadius; ;
        damageText.text = damage.ToString();
    }

    public void SetUI(DamageType damageType, Vector2 position, int damage)
    {
        rectTransform.anchoredPosition = position + Random.insideUnitCircle * appearRadius; ;
        damageText.text = damage.ToString();

        switch (damageType)
        {
            case DamageType.Default:
                damageText.color = Color.red;
                break;
            case DamageType.Critical:
                damageText.color = Color.yellow;
                break;
            case DamageType.Heal:
                damageText.color = Color.green;
                break;
            default:
                break;
        }
    }

    public IEnumerator FloatingEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < floatingTime)
        {
            rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        FloatingDamagePool.Instance.ReturnToPool(gameObject);
    }
}
