using UnityEngine;
using Project.Enums;

public class Spike : MonoBehaviour
{
    [SerializeField] BoxCollider2D DamageCollider;


    [Header("Spike Damage Info")]
    [SerializeField] float DamageAmount = 5f;
    [SerializeField] EStatusEffect EffectType = EStatusEffect.Bleeding;

    [Header("Bleeding Spec")]
    [SerializeField] float Duration = 10f;
    [SerializeField] float Period = 2f;
    //[SerializeField] float EventRate = 0;

    private void Awake()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject Obj = collision.gameObject;
        CharacterBase HitCharacter = Obj.GetComponent<CharacterBase>();

        if(HitCharacter)
        {
            if(!HitCharacter.ActorHasTag(HitCharacter.Tag_Invincible))
            {
                bool StatusEffectEvent = false;
                if(StatusEffectEvent = UtilityLibrary.RandomBool())
                {
                    // 출혈 발생 이벤트는 50%
                    Debug.Log(StatusEffectEvent);
                    HitCharacter.ApplyStatusEffectToSelf(EffectType, Duration, Period);
                }

                UtilityLibrary.OnAttacked_ByObj(HitCharacter, DamageAmount);
            }
        }
    }
}
