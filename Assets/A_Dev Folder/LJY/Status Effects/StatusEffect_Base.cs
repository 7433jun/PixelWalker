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
        // �Ļ� Ŭ�������� �ۼ�
        if(!Target)
        {
            return;
        }
    }

    protected IEnumerator EffectApply(CharacterBase Target, int Stack = 1)
    {
        //Debug.Log("ȿ�� ����");

        if(!Target)
        {
            Debug.Log("NoTarget");
            yield break;
        }

        if(EffectPolicy == EStatusEffectApplyPolicy.OnTriggered)
        {
            // Ʈ���� Ÿ���� ��� ����
            // �ݴ� ������ ���� 
            Effect(Target, Stack);
            yield break;
        }

        else // ��Ʈ ȿ��
        {
            //Debug.Log("��Ʈ ȿ�� ��� ����");
            Effect(Target, Stack); // �ϴ� ��� ȿ�� ����
            float InPeriod = ApplyPeriod;
            while (EffectDuration > 0)
            {
                // ȿ�� ���� �ð��� �����ִ� ���� ���� ����
                if(InPeriod <= 0)
                {
                    //Debug.Log("��Ʈ ȿ�� ����");
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
