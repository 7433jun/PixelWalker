using UnityEngine;

public class GroundAbility_01 : GroundAbilityMasterState
{
    public GroundAbility_01(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if(IsPlayer())
        {
            GetPlayerCharacter().StartBuff_01();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        base.Update();
    }
}
