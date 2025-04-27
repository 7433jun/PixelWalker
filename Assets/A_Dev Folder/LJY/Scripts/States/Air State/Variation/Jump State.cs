using UnityEngine;

public class JumpState : AirMasterState
{
    float JumpToStartTimer = 0;
    const float JumpTimer = .2f;

    float PlayerJumpTime = 0;
    float PlayerJumpMinTime = .1f; // 점프 유지 최소 보장시간

    public JumpState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        OwningCharacter.JumpVelocity();

        OwningCharacter.PlaySoundJump();

        //OwnerRb.linearVelocityY = OwningCharacter.GetPlayerCharacter(OwningCharacter).CharacterSpec.GetJumpForce;

        JumpToStartTimer = 0; // 안전용
        PlayerJumpTime = 0;
    }

    public override void ExitState()
    {
        base.ExitState();

        JumpToStartTimer = 0;
        PlayerJumpTime = 0;
    }

    public override void Update()
    {
        base.Update();

        if (IsPlayer())
        {
            if(PlayerJumpTime < PlayerJumpMinTime)
            {
                PlayerJumpTime += Time.deltaTime;
            }
            else
            {
                // 최소 보장시간 경과 시
                if(!GetPlayerController().PressedJumpKey)
                {
                    GetOwnerRigidBody2D().linearVelocityY -= 0.021f;
                }
            }
        }

        if (JumpToStartTimer < JumpTimer)
        {
            JumpToStartTimer += Time.deltaTime;
        }
        else
        {
            if (OwningCharacter.JumptToSlope)
            {
                OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
            }
        }

        if (GetOwnerRigidBody2D().linearVelocityY < 0)
        {
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._AirborneState);
        }

        if (IsEnemyMob())
        {
            if (GetEnemyCharacter().AttackTarget)
            {
                UtilityLibrary.MoveTo(GetEnemyCharacter(), GetEnemyCharacter().AttackTarget.transform.position, .5f, GetEnemyCharacter().CommonSpec.GetDashSpeed);
            }
        }
    }
}
