using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenSpace : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tilemapRenderer.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tilemapRenderer.enabled = true;
        }
    }
}
