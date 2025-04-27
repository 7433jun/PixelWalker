using UnityEngine;

public class PoisoningEffect : StatusEffect_Base
{
    protected override void Effect(CharacterBase Target, int Stack = 1)
    {
        base.Effect(Target, Stack);

        // 최대 체력의 2%
        float DamageAmount = Target.CommonSpec.GetMaxHP * 0.02f;

        switch (Stack)
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
                Debug.LogWarning("Poisoning Effect Stack Error");
                break;
        }

        Target.CommonSpec.DamageTakenToEffect(DamageAmount);
    }
}
