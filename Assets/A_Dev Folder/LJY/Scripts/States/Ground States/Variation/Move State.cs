using UnityEngine;

public class MoveState : GroundMasterState
{
    Vector2 LastMoveVec;
    const float TurnTimer = 3;
    float ElapsedRun = 0;
    float StopLerpSpeed = 2;

    void TurnRuleCheck()
    {
        if (GetPlayerController().InputVec.x == GetPlayerCharacter().FacingDir && OwningCharacter.IsRunning)
        {
            if(ElapsedRun < TurnTimer)
                ElapsedRun += Time.deltaTime;
        }

        else if(GetPlayerController().InputVec.x != GetPlayerCharacter().FacingDir)
        {
            if (ElapsedRun >= TurnTimer)
            {
                ElapsedRun = 0;

                if(Mathf.Abs(GetOwnerRigidBody2D().linearVelocityX) >= OwningCharacter.CommonSpec.GetDashSpeed && !OwningCharacter.OnSlope)
                {
                    // ���ΰ� �ƴϸ� ��� �ӵ� �̻��̾�߸� ȸ�� ����. �̸��� ��� ���� ���з� ���� | ���� ���� �ʿ������.
                    GetPlayerCharacter().IsTurn = true;
                }
                else
                {
                    return;
                }

                if(GetPlayerController().InputVec.x != 0)
                {
                    // �ݴ� ���� ��ȣ�� ���� ���
                    OwningCharacter.Anim.SetBool("Turn", true);
                }
                else
                {
                    // Ű�� ���� ���
                    OwningCharacter.Anim.SetBool("Dash Break", true);
                }
            }
        }
    }

    public MoveState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        ElapsedRun = 0;

        if(IsPlayer())
            GetPlayerCharacter().IsTurn = false;
    }

    public override void ExitState()
    {
        base.ExitState();

        if (GetOwnerRigidBody2D().linearVelocityX != 0 && IsPlayer())
        {
            if(!GetPlayerCharacter().IsTurn)
                OwningCharacter.ZeroVelocityToX();
        }

        else if(IsEnemyMob())
        {
            OwningCharacter.ZeroVelocityToX();
        }
    }

    public override void Update()
    {
        base.Update();

        if (IsPlayer())
        {
            TurnRuleCheck();

            PlayerMove();
        }

        else if (IsEnemyMob())
        {
            if(GetMobCharacter().AttackTarget && GetMobCharacter().CanChaseToTarget())
            {
                // ��ü��, Ÿ���� �ִ� �Ÿ��� ����, ��ü�� ���� ��Ÿ� -5 ��ŭ, ��� �ӵ��� ����
                float ToleranceDistance = Mathf.Clamp(GetEnemyCharacter().AttackRange - 5, .75f, GetEnemyCharacter().AttackRange);
                // �⺻ ���� �Ÿ��� 5��ŭ ��

                UtilityLibrary.MoveTo(GetEnemyCharacter(), GetEnemyCharacter().AttackTarget.transform.position, ToleranceDistance,
                    GetEnemyCharacter().CommonSpec.GetDashSpeed);
            }

            else
            {
                OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
            }
        }
    }

    void PlayerMove()
    {
        // ȸ�����ΰ� �ƴѰ��� �߿�
        if (!GetPlayerCharacter().IsTurn)
        {
            if (StopLerpSpeed > 2)
                StopLerpSpeed = 2;

            if (GetPlayerCharacter().IsBusy)
            {
                GetPlayerCharacter().IsBusy = false;
            }

            if (GetPlayerCharacter().PCont.InputVec == Vector2.zero)
            {
                OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
            }

            GetPlayerCharacter().HandleNormalMovement();
            LastMoveVec = GetPlayerCharacter().GetMoveVec();
        }

        else
        {
            if (!GetPlayerCharacter().IsBusy)
            {
                GetPlayerCharacter().IsBusy = true;
            }

            StopLerpSpeed += Time.deltaTime;

            float LerpFactor = StopLerpSpeed * Time.deltaTime;
            LastMoveVec.x = Mathf.Lerp(LastMoveVec.x, 0, LerpFactor);

            GetPlayerCharacter().HandleTurningMovement(LastMoveVec.x);
        }
    }
}
