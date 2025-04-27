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
            // 맵 오브젝트들과 충돌시 무조건 바로 파괴
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
                // 파괴 불가 타입 특별 처리가 필요하면 작성
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
                // 특정 캐릭터와 충돌시 일반 탄이면 파괴, 관통탄일 시 무시
                DestroyBullet();
            }
            else if(PenetrateType == EBulletTypes.Penetration)
            {
                // 대상과 충돌할 때마다 사라지는 이펙트 연출
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
