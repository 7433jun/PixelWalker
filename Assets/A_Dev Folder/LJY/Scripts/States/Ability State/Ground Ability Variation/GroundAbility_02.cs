using UnityEngine;

public class GroundAbility_02 : GroundAbilityMasterState
{
    public GroundAbility_02(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        if(IsMinotaur())
        {
            GetMinotaur().Minotaur_SetAbilityRushRange();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        base.Update();
        if(IsPlayer())
        {

        }

        else if(IsBoss())
        {
            // ���� ����
            if(IsMinotaur())
            {
                if(!OwningCharacter.WallDetect())
                {
                    // ���� �����Ǵ� �������� ������ ����
                    OwningCharacter.SetVelocity_Just_XAxis(OwningCharacter.CommonSpec.GetDashSpeed * OwningCharacter.FacingDir);
                }
                else
                {
                    // ���� ������ ���
                    GetMinotaur().SetSwordBoxDeactivate(); // �浹 ���� ����
                    CameraManager.Instance.StartCameraShake(OwningCharacter.CameraShakeProfile, OwningCharacter.ImpulseSource, 
                        GetMinotaur().MinoCS_Rush.ImpulseDuration,
                        GetMinotaur().MinoCS_Rush.ShakeDuration,
                        GetMinotaur().MinoCS_Rush.MaxImpactForce,
                        GetMinotaur().MinoCS_Rush.Magnitude);

                    OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);

                    GetMinotaur().JumpMove_withWait();
                }
            }
        }
    }
}
