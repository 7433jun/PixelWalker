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

        // 피격시 다른 행동 불가
        OwningCharacter.IsBusy = true;

        // 피격 상태 진입시 짧게 무적 시간 부여
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
