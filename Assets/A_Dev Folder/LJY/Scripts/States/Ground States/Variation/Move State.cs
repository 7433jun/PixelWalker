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
                    // 경사로가 아니며 대시 속도 이상이어야만 회전 성립. 미만일 경우 조건 실패로 간주 | 경사로 구분 필요없어짐.
                    GetPlayerCharacter().IsTurn = true;
                }
                else
                {
                    return;
                }

                if(GetPlayerController().InputVec.x != 0)
                {
                    // 반대 방향 신호를 받은 경우
                    OwningCharacter.Anim.SetBool("Turn", true);
                }
                else
                {
                    // 키를 놓은 경우
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
                // 주체가, 타겟이 있는 거리를 향해, 주체의 공격 사거리 -5 만큼, 대시 속도로 접근
                float ToleranceDistance = Mathf.Clamp(GetEnemyCharacter().AttackRange - 5, .75f, GetEnemyCharacter().AttackRange);
                // 기본 접근 거리는 5만큼 줌

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
        // 회전중인가 아닌가가 중요
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
