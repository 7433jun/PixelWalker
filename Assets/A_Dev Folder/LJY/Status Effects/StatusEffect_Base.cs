using UnityEngine;
using Project.Enums;
using System.Collections;

public class StatusEffect_Base : MonoBehaviour
{
    [Header("Generic Options")]
    [SerializeField] protected CharacterBase ApplyTarget;
    [SerializeField] protected EStatusEffectApplyPolicy EffectPolicy = EStatusEffectApplyPolicy.OnTriggered;

    [Header("Dot Type Setting")]
    [SerializeField] protected float EffectDuration;
    [SerializeField] protected float ApplyPeriod;

    public Coroutine DOT_EffectSpecDefinitionAndApply(CharacterBase Target, EStatusEffect NewStatus, float Duration, float Period, int Stack = 1)
    {
        EffectDuration = Duration;
        ApplyPeriod = Period;

        return ApplyStatusEffect(Target, NewStatus, Stack);
    }

    public Coroutine ApplyStatusEffect(CharacterBase Target, EStatusEffect NewStatus, int Stack = 1)
    {
        ApplyTarget = Target;
        ApplyTarget.ChangeStatus(NewStatus);
        return StartCoroutine(EffectApply(Target, Stack));
    }

    protected virtual void Effect(CharacterBase Target, int Stack = 1)
    {
        // 파생 클래스에서 작성
        if(!Target)
        {
            return;
        }
    }

    protected IEnumerator EffectApply(CharacterBase Target, int Stack = 1)
    {
        //Debug.Log("효과 적용");

        if(!Target)
        {
            Debug.Log("NoTarget");
            yield break;
        }

        if(EffectPolicy == EStatusEffectApplyPolicy.OnTriggered)
        {
            // 트리거 타입은 즉시 적용
            // 반대 유형의 경우는 
            Effect(Target, Stack);
            yield break;
        }

        else // 도트 효과
        {
            //Debug.Log("도트 효과 즉시 적용");
            Effect(Target, Stack); // 일단 즉시 효과 적용
            float InPeriod = ApplyPeriod;
            while (EffectDuration > 0)
            {
                // 효과 유지 시간이 남아있는 동안 지속 실행
                if(InPeriod <= 0)
                {
                    //Debug.Log("도트 효과 적용");
                    Effect(Target, Stack);
                    InPeriod = ApplyPeriod;
                }

                InPeriod -= Time.deltaTime;
                EffectDuration -= Time.deltaTime;
                yield return null;
            }
        }

        TargetStatusReset(Target);
        yield return null;
    }

    void TargetStatusReset(CharacterBase Target)
    {
        Target.ChangeStatus(EStatusEffect.None);
    }
}
