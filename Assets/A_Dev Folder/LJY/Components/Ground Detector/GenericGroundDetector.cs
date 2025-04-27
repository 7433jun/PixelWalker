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
    [SerializeField] Vector2 BoxSize = new Vector2(0.5f, 0.1f); // 박스 크기
    [SerializeField] float BoxOffsetX = 0; // 박스의 X축 위치 오프셋
    [SerializeField] float BoxOffsetY = -0.5f; // 박스의 Y축 위치 오프셋
    [SerializeField] float BoxCastDistance = 0.1f; // BoxCast 거리

    public bool GroundDetecting()
    {
        Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

        RaycastHit2D HitResult;
        // BoxCast 수행

        HitResult = Physics2D.BoxCast(
            BoxStart,        // 박스 시작 위치
            BoxSize,         // 박스 크기
            0f,              // 박스 회전 각도 (2D에서는 0)
            Vector2.down,    // 박스 캐스팅 방향 (아래로)
            BoxCastDistance, // 캐스팅 거리
            GroundLayer      // 충돌 감지할 레이어
        );

        if(bUseOption_PlatformSearch && !HitResult) // 플랫폼 서치를 허용하면서 땅을 못찾은 경우
        {
            HitResult = Physics2D.BoxCast(
            BoxStart,        // 박스 시작 위치
            BoxSize,         // 박스 크기
            0f,              // 박스 회전 각도 (2D에서는 0)
            Vector2.down,    // 박스 캐스팅 방향 (아래로)
            BoxCastDistance, // 캐스팅 거리
            PlatformLayer      // 충돌 감지할 레이어
            );
        }

        return HitResult.collider != null;
    }

    public float GetGroundDistance()
    {
        Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

        // BoxCast 수행
        RaycastHit2D hit = Physics2D.BoxCast(
            BoxStart,        // 박스 시작 위치
            BoxSize,         // 박스 크기
            0f,              // 박스 회전 각도
            Vector2.down,    // 캐스팅 방향
            BoxCastDistance, // 캐스팅 거리
            GroundLayer      // 충돌 감지 레이어
        );

        if (hit.collider != null)
        {
            return hit.distance; // 충돌한 지점까지의 거리 반환
        }

        return float.MaxValue; // 충돌이 없으면 큰 값 반환
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
            // 박스 시작 위치 계산
            Vector2 BoxStart = (Vector2)transform.position + new Vector2(BoxOffsetX, BoxOffsetY);

            // Gizmos 색상 설정
            Gizmos.color = Color.green;

            // 시작 박스 표시
            Gizmos.DrawWireCube(BoxStart, BoxSize);

            // 캐스트 종료 지점 표시
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
