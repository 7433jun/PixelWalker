using UnityEngine;

public class GroundMasterState : MasterAnimState
{
    public GroundMasterState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        if ((IsPlayer() || IsEnemyMob()) && GetOwnerRigidBody2D().gravityScale != 1)
        {
            // �÷��̾�� ����� ����
            GetOwnerRigidBody2D().gravityScale = 1;
        }

        if (IsPlayer())
        {
            if (GetPlayerCharacter().PlayerSpec.CanDoubleJump)
            {
                GetPlayerController().ResetAdditiveJump();
            }

            if (OwningCharacter.bIsDoubleJump)
            {
                OwningCharacter.SetDoubleJumpCondition(false);
            }
        }

    }

    public override void ExitState()
    {
        base.ExitState();

        if(!OwningCharacter.IsDamaged)
        {
            GetOwnerRigidBody2D().linearVelocityY = 0;
        }
    }

    public override void Update()
    {
        base.Update();

        if(OwningCharacter.IsDamaged)
        {
            //OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._DamagedState);
        }

        if (OwningCharacter.GroundDetectorComp.GetGroundDistance() >= 0.02f)
        {
            // ���� ���� �ɼ� (�� �ߴ� ���� ����)
            GetOwnerRigidBody2D().linearVelocityY = -3;
        }

        if (!OwningCharacter.GroundDetect())
        {
            // ���� �پ��ִٰ� ���ڱ� �ٴ��� �������ٸ�?

            if (OwningCharacter.CompareTag("Player"))
            {
                if (GetPlayerCharacter().IsBusy)
                {
                    GetPlayerCharacter().IsBusy = false;
                }

                if (GetPlayerCharacter().IsTurn)
                {
                    GetPlayerCharacter().IsTurn = false;
                }
            }

            // ��� ������Ʈ�� ����
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._AirborneState);
        }
    }
}
