using UnityEditor.Tilemaps;
using UnityEngine;

public class AirMasterState : MasterAnimState
{
    float AirborneSpeed;

    public AirMasterState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        if (OwningCharacter.CompareTag("Player"))
        {
            if (GetPlayerCharacter().IsRunning)
                AirborneSpeed = OwningCharacter.CommonSpec.GetDashSpeed;

            else
                AirborneSpeed = OwningCharacter.CommonSpec.GetWalkSpeed;
        }
    }

    public override void ExitState()
    {
        base.ExitState();

    }

    public override void Update()
    {
        base.Update();

        if (OwningCharacter.CompareTag("Player"))
        {
            if (GetPlayerCharacter().PCont.InputVec.x != 0)
            {
                OwningCharacter.SetVelocity_XAxis_WithFlip(GetPlayerCharacter().PCont.InputVec * AirborneSpeed);
            }
        }

        if(IsBoss())
        {
            UtilityLibrary.MoveTo(OwningCharacter, GetBossCharacter().NewMove, 0, OwningCharacter.CommonSpec.GetWalkSpeed);
        }
    }
}
