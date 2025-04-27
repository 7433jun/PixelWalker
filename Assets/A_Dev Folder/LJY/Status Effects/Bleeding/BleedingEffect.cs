using UnityEngine;

public class BleedingEffect : StatusEffect_Base
{
    protected override void Effect(CharacterBase Target, int Stack)
    {
        base.Effect(Target, Stack);

        // 최대 체력의 1%
        float DamageAmount = Target.CommonSpec.GetMaxHP * 0.01f;

        switch(Stack)
        {
            case 2:
                DamageAmount *= 1.5f;
                break;

            case 3:
                DamageAmount *= 2f;
                break;

            case 4:
                DamageAmount *= 4f;
                break;

            default:
                Debug.LogWarning("Bleeding Effect Stack Error");
                break;
        }

        Target.CommonSpec.DamageTakenToEffect(DamageAmount);
    }
}
