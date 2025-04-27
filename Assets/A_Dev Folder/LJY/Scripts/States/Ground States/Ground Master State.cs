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
            // 플레이어와 잡몹만 적용
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
            // 경사로 전용 옵션 (붕 뜨는 현상 방지)
            GetOwnerRigidBody2D().linearVelocityY = -3;
        }

        if (!OwningCharacter.GroundDetect())
        {
            // 땅에 붙어있다가 갑자기 바닥이 없어진다면?

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

            // 에어본 스테이트로 변경
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._AirborneState);
        }
    }
}
