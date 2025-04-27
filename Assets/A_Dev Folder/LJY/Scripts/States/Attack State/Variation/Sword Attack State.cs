using UnityEngine;

public class MeleeAttackState : PrimaryAttackMasterState
{
    public MeleeAttackState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        int CurCount = OwningCharacter.SwordAttackCount++;

        GetOwningCharacterAnimator().SetInteger("Sword Combo Count", CurCount);
    }

    public override void ExitState()
    {
        base.ExitState();

        if(OwningCharacter.SwordAttackCount == OwningCharacter.HasPrimaryAttackCountToSword) // ������ ���ݱ��� ������ �ʱ�ȭ
        {
            OwningCharacter.SwordAttackCount = 0;
        }
    }

    public override void Update()
    {
        base.Update();

    }
}
