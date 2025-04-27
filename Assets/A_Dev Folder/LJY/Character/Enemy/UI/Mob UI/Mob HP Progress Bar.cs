using Unity.Cinemachine;
using UnityEngine;

public class MobHPProgressBar : MonoBehaviour
{
    public CharacterBase Owner;
    public Canvas CanvasPanel;
    public Vector2 CanvasOffset;

    private RectTransform RectTransform;

    void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasOffset = Owner.GetComponent<MobBase>().MobHPSliderOffset;

        GameObject FoundCanvas = GameObject.Find("Floating Canvas");
        CanvasPanel = FoundCanvas.GetComponent<Canvas>();

        gameObject.SetActive(false);
    }


    void FixedUpdate()
    {
        SetWidgetPosition();
    }

    void SetWidgetPosition()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(Owner.transform.position);

        Vector2 canvasPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasPanel.GetComponent<RectTransform>(), screenPoint, CanvasPanel.worldCamera, out canvasPoint);

        RectTransform.anchoredPosition = canvasPoint + CanvasOffset;
    }
}
