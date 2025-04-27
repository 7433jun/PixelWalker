using UnityEngine;

public class MapUI : MonoBehaviour, Iui
{
    private Canvas canvas;
    public Canvas Canvas => canvas;

    void Start()
    {
        GameManager.UI.SetIui(this);

        canvas = GetComponent<Canvas>();

        UpdateLanguage();
    }

    public void UpdateLanguage()
    {

    }
}
