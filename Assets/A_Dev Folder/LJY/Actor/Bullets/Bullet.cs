using Unity.Cinemachine;
using UnityEngine;
using Project.Enums;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public CharacterBase Owner;

    public EBulletDestroyTypes BulletDestroyType = EBulletDestroyTypes.Destructible;
    public EBulletTypes PenetrateType = EBulletTypes.Normal;
    [SerializeField] EProjectileType BulletType;

    [SerializeField] float BulletSpeed;
    public float LifePeriod;
    [HideInInspector] public float LifeTime;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    [HideInInspector] public float FireDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * BulletSpeed * FireDir;
    }

    private void Update()
    {
        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject ColliderOwner = collision.gameObject;

        if (ColliderOwner.CompareTag("Ground"))
        {
            // �� ������Ʈ��� �浹�� ������ �ٷ� �ı�
            DestroyBullet();
        }

        if (ColliderOwner.CompareTag("Melee Range"))
        {
            if(BulletDestroyType == EBulletDestroyTypes.Destructible)
            {
                DestroyBullet();
                return;
            }
            else
            {
                // �ı� �Ұ� Ÿ�� Ư�� ó���� �ʿ��ϸ� �ۼ�
            }
        }

        CharacterBase Instigator = ColliderOwner.GetComponent<CharacterBase>();

        if (ColliderOwner.CompareTag("Player") || ColliderOwner.CompareTag("Enemy"))
        {
            CharacterBase DamagedTarget = collision.gameObject.GetComponent<CharacterBase>();

            if (!UtilityLibrary.IsHostile(Owner, DamagedTarget))
            {
                Debug.Log("Is Not Hostile!");

                return;
            }

            if (ColliderOwner.CompareTag("Player"))
            {
                //Debug.Log("Bullet Player Hit");
            }

            else if (ColliderOwner.CompareTag("Enemy"))
            {
                //Debug.Log("Bullet Enemy Hit");
            }

            UtilityLibrary.OnAttacked_InstigatorToTarget(Owner, DamagedTarget, this.gameObject, Owner.AbilitySpec.GetBulletDamageRate);

            if(PenetrateType == EBulletTypes.Normal)
            {
                // Ư�� ĳ���Ϳ� �浹�� �Ϲ� ź�̸� �ı�, ����ź�� �� ����
                DestroyBullet();
            }
            else if(PenetrateType == EBulletTypes.Penetration)
            {
                // ���� �浹�� ������ ������� ����Ʈ ����
                SpawnEmitter(transform.position);
            }
        }
    }

    public void SpawnEmitter(Vector2 Location)
    {
        GameObject BulletVFXToSpawn = BulletPool.BP_Instance.GetBulletVFXTypeByBulletType(BulletType);

        if (UtilityLibrary.IsValid(BulletVFXToSpawn))
        {
            BulletVFXToSpawn.transform.position = Location;
            BulletVFXToSpawn.SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid Bullet VFX");
        }
    }

    public void DestroyBullet()
    {
        SpawnEmitter(transform.position);
        gameObject.SetActive(false);
    }
}
