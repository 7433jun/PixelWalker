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
            // ���� �ݻ� ��͵� ����
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
                // �޺��� �ִ� ĳ���Ϳ��Ը� ��ȿ. ����� �÷��̾ �޺��� ����
            }

            else if (Owner.CompareTag("Enemy"))
            {
                UtilityLibrary.OnAttacked_InstigatorToTarget(Owner, DamagedTarget, Owner.gameObject, Owner.AbilitySpec.GetPrimarySwordComboDamageRate(DamageRateIdx));
                // �� ������ ��� �޺��� �������� ����.
                // ��, ����� �ٸ� ���ݵ ���� ���ط� ������ ���� ���� �ε��� Ȱ��
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
