using UnityEngine;

public class FloatingSlider : MonoBehaviour
{
    public Canvas canvas;
    public Transform target;
    public Vector2 canvasOffset;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    
    void FixedUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.position);

        Vector2 canvasPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out canvasPoint);

        rectTransform.anchoredPosition = canvasPoint + canvasOffset;
    }
}
