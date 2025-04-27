using System.Collections;
using UnityEngine;

public class DamagedState : MasterAnimState
{
    public DamagedState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if(IsPlayer())
        {
            GetPlayerCharacter().InBattleState(true);
        }

        // �ǰݽ� �ٸ� �ൿ �Ұ�
        OwningCharacter.IsBusy = true;

        // �ǰ� ���� ���Խ� ª�� ���� �ð� �ο�
        OwningCharacter.EnableInvincible();

        OwningCharacter.ClearInvincible(OwningCharacter.CommonSpec.GetHitInvincibleTime);
    }

    public override void ExitState()
    {
        base.ExitState();

        OwningCharacter.IsBusy = false;
        OwningCharacter.IsDamaged = false;
    }

    public override void Update()
    {
        base.Update();
    }

}
