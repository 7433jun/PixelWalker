using UnityEngine;

public class PrimaryAttackMasterState : MasterAnimState
{
    public PrimaryAttackMasterState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if(IsPlayer())
        {
            GetPlayerCharacter().InBattleState(true);
        }

        if(GetOwnerRigidBody2D().linearVelocityX != 0)
            OwningCharacter.ZeroVelocityToX();

        OwningCharacter.IsBusy = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        OwningCharacter.IsBusy = false;
    }

    public override void Update()
    {
        base.Update();

        
    }
}
