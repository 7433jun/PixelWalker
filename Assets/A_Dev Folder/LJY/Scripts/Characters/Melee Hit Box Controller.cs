using Project.Structs;
using UnityEngine;
using Project.Enums;

public class MeleeHitBoxController : MonoBehaviour
{
    [SerializeField] CharacterBase Owner;
    BoxCollider2D MeleeBox;
    [HideInInspector] public int DamageRateIdx = 0;

    public void SetMeleeBoxActivate(bool Condition)
    {
        MeleeBox.enabled = Condition;
    }

    public void SetMeleeBoxSize(FSwordHitBox OffsetData)
    {
        MeleeBox.offset = OffsetData.Offset;
        MeleeBox.size = OffsetData.Size;
    }

    private void Awake()
    {
        Owner = GetComponentInParent<CharacterBase>();
        MeleeBox = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        MeleeBox.enabled = false;
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject ColliderOwner = collision.gameObject;

        if(ColliderOwner.CompareTag("Bullet"))
        {
            // 이후 반사 기믹도 구현
            Bullet HitBullet = ColliderOwner.GetComponent<Bullet>();

            if (HitBullet.BulletDestroyType == EBulletDestroyTypes.InDestructible)
            {
                return;
            }
        }

        if (ColliderOwner.CompareTag("Player") || ColliderOwner.CompareTag("Enemy"))
        {
            CharacterBase DamagedTarget = collision.gameObject.GetComponent<CharacterBase>();

            if(DamagedTarget.IsDead)
            {
                return;
            }

            if (Owner.CompareTag("Player"))
            {
                Owner.CommonSpec.MPGain();
                UtilityLibrary.OnAttacked_InstigatorToTarget(Owner, DamagedTarget, Owner.gameObject, Owner.AbilitySpec.GetPrimarySwordComboDamageRate(Owner.SwordAttackCount - 1));
                // 콤보가 있는 캐릭터에게만 유효. 현재는 플레이어만 콤보가 있음
            }

            else if (Owner.CompareTag("Enemy"))
            {
                UtilityLibrary.OnAttacked_InstigatorToTarget(Owner, DamagedTarget, Owner.gameObject, Owner.AbilitySpec.GetPrimarySwordComboDamageRate(DamageRateIdx));
                // 적 유닛의 경우 콤보가 존재하지 않음.
                // 단, 모션이 다른 공격등에 한해 피해량 차등을 위해 전용 인덱스 활용
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
