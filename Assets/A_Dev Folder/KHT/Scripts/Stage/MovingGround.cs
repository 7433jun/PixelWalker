using System.Collections;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    public Vector2 pointA;
    public Vector2 pointB;
    public float speed = 2.0f;
    public float waitTime = 2.0f;

    void Start()
    {
        StartCoroutine(MoveBetweenPoints());
    }

    IEnumerator MoveBetweenPoints()
    {
        Vector2 targetPoint = pointA;

        while (true)
        {
            while(Vector2.Distance(transform.position, targetPoint) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            targetPoint = targetPoint == pointA ? pointB : pointA;
        }
    }
}
