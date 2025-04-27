using UnityEngine;

public class ForwardTopographyDetector : MonoBehaviour
{
    [Header("Detector Option")]
    [SerializeField] bool bUseDrawDebug;
    //[SerializeField] Transform DetectingPoint;
    [SerializeField] float DetectLength;
    [SerializeField] LayerMask SearchTopography;

    //[Header("Ground Search Condition")]
    //[SerializeField] bool bSearchedGround;

    private void Start()
    {
        
    }

    private void Update()
    {
       // bSearchedGround = SearchedForwardGround();
    }

    private void OnDrawGizmos()
    {
        if(bUseDrawDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * DetectLength);
        }
    }

    public bool SearchedForwardGround()
    {
        RaycastHit2D HitResult = Physics2D.Raycast(transform.position, Vector3.down, DetectLength, SearchTopography);

        if (HitResult)
        {
            return true;
        }

        return false;
    }
}
