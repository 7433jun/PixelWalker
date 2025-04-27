using Project.Enums;
using UnityEngine;

public class StoneWall : MonoBehaviour
{
    #region Component
    [SerializeField] CharacterBase Owner;
    BoxCollider2D HitBox;
    public Animator Anim;
    #endregion

    #region Notify
    public void EnableHitBox()
    {
        HitBox.enabled = true;
    }

    public void DisableHitBox()
    {
        HitBox.enabled = false;
    }

    public void EnableActivate()
    {
        gameObject.SetActive(true);
    }

    public void DisableActivate()
    {
        gameObject.SetActive(false);
    }
    #endregion

    private void Awake()
    {
        Owner = GetComponentInParent<CharacterBase>();
        HitBox = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();   
    }
    void Start()
    {
        EnableActivate();
        EnableHitBox();
    }
    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject ColliderOwner = collision.gameObject;

        if (ColliderOwner.CompareTag("Bullet"))
        {
            Bullet HitBullet = ColliderOwner.GetComponent<Bullet>();

            if (HitBullet.BulletDestroyType == EBulletDestroyTypes.InDestructible)
            {
                HitBullet.DestroyBullet(); // º®¿¡ ¹ÚÈù ÃÑ¾ËÀº Áï°¢ ÆÄ±«
                return;
            }
        }

        if (ColliderOwner.CompareTag("Player") || ColliderOwner.CompareTag("Enemy"))
        {
            CharacterBase DamagedTarget = collision.gameObject.GetComponent<CharacterBase>();

            if (DamagedTarget.IsDead)
            {
                return;
            }

            if (Owner.CompareTag("Player"))
            {
                //Owner.CommonSpec.MPGain();
            }

            else if (Owner.CompareTag("Enemy"))
            {

            }

            UtilityLibrary.OnAttacked_InstigatorToTarget(Owner, DamagedTarget, Owner.gameObject, Owner.AbilitySpec.GetStoneWallRate);
        }
    }
}
