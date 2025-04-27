using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GenericGroundDetector : MonoBehaviour
{
    CharacterBase Owner;
    [SerializeField] bool bUseOption_PlatformSearch = true;
    
    LayerMask GroundLayer;
    LayerMask PlatformLayer;

    [SerializeField] bool bUseDrawDebug;

    private void Start()
    {
        Owner = GetComponentInParent<CharacterBase>();
        GroundLayer = LayerMask.GetMask("Ground");
        PlatformLayer = LayerMask.GetMask("Platform");
    }

    private void Update()
    {

    }

    public void FlipDetectorOffset()
    {
        BoxOffsetX *= -1;
    }

    #region Ground Detector
    [Header("Ground Detect")]
    [SerializeField] Vector2 BoxSize = new Vector2(0.5f, 0.1f); // �ڽ� ũ��
    [SerializeField] float BoxOffsetX = 0; // �ڽ��� X�� ��ġ ������
    [SerializeField] float BoxOffsetY = -0.5f; // �ڽ��� Y�� ��ġ ������
    [SerializeField] float BoxCastDistance = 0.1f; // BoxCast �Ÿ�

    public bool GroundDetecting()
    {
        Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

        RaycastHit2D HitResult;
        // BoxCast ����

        HitResult = Physics2D.BoxCast(
            BoxStart,        // �ڽ� ���� ��ġ
            BoxSize,         // �ڽ� ũ��
            0f,              // �ڽ� ȸ�� ���� (2D������ 0)
            Vector2.down,    // �ڽ� ĳ���� ���� (�Ʒ���)
            BoxCastDistance, // ĳ���� �Ÿ�
            GroundLayer      // �浹 ������ ���̾�
        );

        if(bUseOption_PlatformSearch && !HitResult) // �÷��� ��ġ�� ����ϸ鼭 ���� ��ã�� ���
        {
            HitResult = Physics2D.BoxCast(
            BoxStart,        // �ڽ� ���� ��ġ
            BoxSize,         // �ڽ� ũ��
            0f,              // �ڽ� ȸ�� ���� (2D������ 0)
            Vector2.down,    // �ڽ� ĳ���� ���� (�Ʒ���)
            BoxCastDistance, // ĳ���� �Ÿ�
            PlatformLayer      // �浹 ������ ���̾�
            );
        }

        return HitResult.collider != null;
    }

    public float GetGroundDistance()
    {
        Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

        // BoxCast ����
        RaycastHit2D hit = Physics2D.BoxCast(
            BoxStart,        // �ڽ� ���� ��ġ
            BoxSize,         // �ڽ� ũ��
            0f,              // �ڽ� ȸ�� ����
            Vector2.down,    // ĳ���� ����
            BoxCastDistance, // ĳ���� �Ÿ�
            GroundLayer      // �浹 ���� ���̾�
        );

        if (hit.collider != null)
        {
            return hit.distance; // �浹�� ���������� �Ÿ� ��ȯ
        }

        return float.MaxValue; // �浹�� ������ ū �� ��ȯ
    }
    #endregion

    #region Slope Detector
    [Header("Slope Detect")]
    [SerializeField] float SlopeCastDistance = 1;
    Vector2 Perpendicular;
    float Angle;

    public bool SlopeDetecting()
    {
        RaycastHit2D HitResult = Physics2D.Raycast(transform.position, Vector2.down, SlopeCastDistance, GroundLayer);

        Perpendicular = Vector2.Perpendicular(HitResult.normal);
        Angle = Vector2.Angle(HitResult.normal, Vector2.up);

        if (Owner.bUseDrawDebug)
        {
            Debug.DrawLine(HitResult.point, HitResult.point + HitResult.normal, Color.red);
            Debug.DrawLine(HitResult.point, HitResult.point + Perpendicular, Color.blue);
        }

        if (Angle != 0)
            return true;

        else
            return false;
    }
    #endregion

    #region Wall Detector
    [Header("Wall Detect")]
    [SerializeField] float DetectLength = 1;
    [SerializeField] Vector2 DetectOffset;

    public bool WallDetecting()
    {
        Vector2 StartPoint = (Vector2) Owner.transform.position + DetectOffset;
        Vector2 DetectDir = (Vector2.right * Owner.FacingDir);

        RaycastHit2D HitResult = Physics2D.Raycast(StartPoint, DetectDir, DetectLength, GroundLayer);

        if(HitResult)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    void OnDrawGizmos()
    {
        if (bUseDrawDebug)
        {
            #region Ground Detect Debug
            // �ڽ� ���� ��ġ ���
            Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

            // Gizmos ���� ����
            Gizmos.color = Color.green;

            // ���� �ڽ� ǥ��
            Gizmos.DrawWireCube(BoxStart, BoxSize);

            // ĳ��Ʈ ���� ���� ǥ��
            Vector2 castEnd = BoxStart + Vector2.down * BoxCastDistance;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(castEnd, BoxSize);
            #endregion

            #region Wall Detect Debug
            if(Owner)
            {
                Vector2 StartPoint = (Vector2) Owner.transform.position + DetectOffset;
                Vector2 DetectDir = StartPoint + (Vector2.right * Owner.FacingDir * DetectLength);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(StartPoint, DetectDir);
            }

            #endregion
        }
    }
}
