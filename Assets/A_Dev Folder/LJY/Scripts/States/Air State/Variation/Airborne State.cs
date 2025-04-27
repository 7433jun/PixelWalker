using UnityEngine;

public class AirborneState : AirMasterState
{

    public AirborneState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        // 에어본 상태 진입시 받는 중력 영향 2배
        if((IsPlayer() || IsEnemyMob()))
        {
            // 플레이어와 잡몹만
            GetOwnerRigidBody2D().gravityScale = 2;
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        if ((IsPlayer() || IsEnemyMob()))
        {
            GetOwnerRigidBody2D().gravityScale = 1;
        }

        if (IsBoss())
        {
            if(GetBossCharacter().TargetIsBack())
            {
                OwningCharacter.Flip();
            }
        }
    }

    public override void Update()
    {
        base.Update();

        // ** 공용 ** //
        if (OwningCharacter.GroundDetect() && !OwningCharacter.ActorHasTag(OwningCharacter.Tag_DetectorDisable))
        {
            // 착지 시 대기 전환
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
        }
    }
}
