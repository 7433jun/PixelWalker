using UnityEngine;

public class AirborneState : AirMasterState
{

    public AirborneState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        // ��� ���� ���Խ� �޴� �߷� ���� 2��
        if((IsPlayer() || IsEnemyMob()))
        {
            // �÷��̾�� �����
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

        // ** ���� ** //
        if (OwningCharacter.GroundDetect() && !OwningCharacter.ActorHasTag(OwningCharacter.Tag_DetectorDisable))
        {
            // ���� �� ��� ��ȯ
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
        }
    }
}
