using UnityEngine;

public class IdleState : GroundMasterState
{
    float IdleTimer = 0;
    const float LongIdleTime = 7f;

    public IdleState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        IdleTimer = 0;

        if (GetOwnerRigidBody2D().linearVelocity != Vector2.zero)
        {
            OwningCharacter.ZeroVelocity();
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        if (OwningCharacter.HasIdleAnims > 1) // 기본값인 1개보다 많을 때만
        {
            if(GetOwningCharacterAnimator().GetBool("Idle Maintain"))
            GetOwningCharacterAnimator().SetBool("Idle Maintain", false);
        }
    }

    public override void Update()
    {
        base.Update();

        if(GetOwnerRigidBody2D().linearVelocityX != 0)
        {
            // 만약 이동 발판등의 경우가 생긴다면 조건 추가
            OwningCharacter.ZeroVelocityToX();
        }

        if(IdleTimer < LongIdleTime && OwningCharacter.HasIdleAnims > 1)
        {
            if (!GetOwningCharacterAnimator().GetBool("Idle Maintain"))
                IdleTimer += Time.deltaTime;
        }
        
        else if(IdleTimer >= LongIdleTime && !GetOwningCharacterAnimator().GetBool("Idle Maintain"))
        {
            PlaySubAnim("Idle List", OwningCharacter.HasIdleAnims, "Idle Maintain", ref IdleTimer);
        }

        if (OwningCharacter.CompareTag("Player"))
        {
            if (GetPlayerCharacter().PCont.InputVec != Vector2.zero)
            {
                // MoveState
                OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._MoveState);
            }
        }
    }

}
