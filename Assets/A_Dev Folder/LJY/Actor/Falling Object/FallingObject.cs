using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingObject : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Collider2D HitCollider;
    [SerializeField] CharacterBase SpawnedOwner;
    Animator Anim;

    [Header("Ground Detector")]
    [SerializeField] bool bUseDrawDebug;
    [SerializeField] Transform CheckStartPoint;
    [SerializeField] bool bIsGround;
    [SerializeField] float DetectLength;
    LayerMask GroundLayer;

    float LifeTime = 4;
    float ElapsedTime = 0;

    public void ObjectIdentify(CharacterBase Instigator, float AdditiveScale)
    {
        SpawnedOwner = Instigator;
        ElapsedTime = 0;
        DetectLength = 0.6f;
        DetectLength *= AdditiveScale;
    }

    #region Ground Detector
    private void OnDrawGizmos()
    {
        if(bUseDrawDebug)
        {
            Gizmos.DrawLine(CheckStartPoint.position, new Vector3(CheckStartPoint.position.x, CheckStartPoint.position.y - DetectLength));
        }
    }

    bool GroundDetect() => Physics2D.Raycast(CheckStartPoint.position, Vector2.down, DetectLength, GroundLayer);
    #endregion

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GroundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        bIsGround = GroundDetect();

        if (bIsGround)
        {
            DestroyActor();
        }

        if (LifeTime > ElapsedTime)
        {
            ElapsedTime += Time.deltaTime;
        }
        else
        {
            DestroyActor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject HitResult = collision.gameObject;
        CharacterBase HitCharacter = HitResult.GetComponentInChildren<CharacterBase>();

        if(HitCharacter == SpawnedOwner)
        {
            return;
        }

        if (UtilityLibrary.IsHostile(SpawnedOwner, HitCharacter))
        {
            UtilityLibrary.OnAttacked_InstigatorToTarget(SpawnedOwner, HitCharacter, SpawnedOwner.gameObject, SpawnedOwner.CommonSpec.GetFinalATK / 2);
            // 피해량은 최종 공격력의 절반만큼으로 고정
            DestroyActor();
        }
    }

    void DestroyActor()
    {
        Anim.SetTrigger("Break");
    }

    public void DisableActivate()
    {
        gameObject.SetActive(false);
    }
}
