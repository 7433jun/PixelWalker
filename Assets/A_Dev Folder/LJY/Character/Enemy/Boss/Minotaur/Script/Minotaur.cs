using Project.Structs;
using Unity.VisualScripting;
using UnityEngine;
using Project.Enums;

public class Minotaur : BossBase
{
    StoneWallController SWC;

    #region Camera Shake Offsets
    public FCameraShakeOffset MinoCS_Rush;
    public FCameraShakeOffset MinoCS_BeatGround_Last;
    public FCameraShakeOffset MinoCS_BeatGround;
    #endregion

    #region Ability Notify
    public void Minotaur_SpawnStoneWall()
    {
        SWC.StartSpawnWall();
    }

    public void Minotaur_SetAbilityMeleeRange()
    {
        MeleeHitBox_Controller.DamageRateIdx = 1;
        MeleeHitBox_Controller.SetMeleeBoxActivate(true);
        // 이미 공격을 실행하고 노티파이로 바꾸는거라 카운트 -1
        MeleeHitBox_Controller.SetMeleeBoxSize(SwordHitBoxOffsetAndSize[1]);
    }
    public void Minotaur_SetAbilityRushRange()
    {
        MeleeHitBox_Controller.DamageRateIdx = 2;
        MeleeHitBox_Controller.SetMeleeBoxActivate(true);
        // 이미 공격을 실행하고 노티파이로 바꾸는거라 카운트 -1
        MeleeHitBox_Controller.SetMeleeBoxSize(SwordHitBoxOffsetAndSize[2]);
    }
    #endregion

    #region Engine Func
    protected override void Awake()
    {
        base.Awake();
        SWC = GetComponentInChildren<StoneWallController>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 5f;

        UtilityLibrary.SetCameraShakeOffset(ref MinoCS_Rush, 0.15f, 1f, 0.5f, .25f);
        UtilityLibrary.SetCameraShakeOffset(ref MinoCS_BeatGround_Last, 0.1f, 0.5f, 0.3f, .25f);
        UtilityLibrary.SetCameraShakeOffset(ref MinoCS_BeatGround, 0.1f, 0.1f, 0.1f, .25f);
    }

    protected override void Update()
    {
        base.Update();

        DebugSet();

        if(StartBattle)
            RunBehaivior();
    }
    #endregion

    #region Behaviour Supporter

    public void Mino_StartIntro()
    {
        AppearBoss();
    }

    public void Mino_Notify_BattleStart()
    {
        StartBattle = true;
    }

    #endregion

    void DebugSet()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            if (StartBattle)
            {
                Debug.LogWarning("디버그용 보스 AI 강제 중단");
                StartBattle = false;
            }
        }
        if (Input.GetKey(KeyCode.F2))
        {
            Debug.Log("Minotaur_ Boss Intro");
            AppearBoss();
        }
        if (Input.GetKey(KeyCode.F3))
        {
            JumpMove_NoWait();
        }
        if (Input.GetKey(KeyCode.F4))
        {
            Debug.Log("Minotaur_ Run AI");
            StartBattle = true;
        }
        if (Input.GetKey(KeyCode.F5))
        {
            Mino_RushAbility();
        }
        if(Input.GetKey(KeyCode.F6))
        {
            Mino_SpecialAbility();
        }
        if (Input.GetKey(KeyCode.F7))
        {
            Mino_PrimaryAttack();
        }
        if (Input.GetKey(KeyCode.F8))
        {
            Mino_FallingRock();
        }
    }

    #region Ability Task
    void Mino_SpecialAbility()
    {
        // 공격 패턴 최상위 우선도
        // 내려치기 바위 기둥 생성
        Debug.Log("1 패턴");
        // 쿨타임 적용
        StartCoolDown(ref P1_CoolTimer, ref Pattern_01);
        UsingStateMachine.ChangeState(_GroundAbility_01);

        // 행동 종료 후 대기 실행
        Wait(3);
    }

    void Mino_RushAbility()
    {
        // 공격 패턴 우선도 2순위
        // 돌진 공격
        Debug.Log("돌진!");
        // 쿨타임 적용
        StartCoolDown(ref P2_CoolTimer, ref Pattern_02);
        UsingStateMachine.ChangeState(_GroundAbility_02);
    }

    void Mino_PrimaryAttack()
    {
        // 공격 패턴 최하위 우선도
        // 공격을 무한정하면 안되니 이후 쿨타임 적용
        Debug.Log("평타 패턴");
        // 일반 공격 개시
        UsingStateMachine.ChangeState(_MeleeAttackState);
        // 쿨타임 적용
        StartCoolDown(ref PA_CoolTimer, ref Primary_Attack);
    }

    void Mino_FallingRock()
    {
        // 공격 패턴 최하위 우선도
        // 공격을 무한정하면 안되니 이후 쿨타임 적용
        Debug.Log("낙석 소환");
        // 공격 개시
        UsingStateMachine.ChangeState(_GroundAbility_03);
        // 쿨타임 적용
        StartCoolDown(ref P3_CoolTimer, ref Pattern_03);
    }

    public void Mino_Notify_BeatGround_CameraShake()
    {
        UtilityLibrary.PlayCameraShake(CameraShakeProfile, ImpulseSource, MinoCS_BeatGround);
    }

    public void Mino_Notify_BeatGround_Last_CameraShake()
    {
        UtilityLibrary.PlayCameraShake(CameraShakeProfile, ImpulseSource, MinoCS_BeatGround_Last);
    }

    public void Mino_Notify_SpawnFallingRock(float Scale = 1)
    {
        GameObject FallingRockToSpawn = FallingObjectPool.Instance.GetFallingObjectByObjectType(EFallingObjectType.Rock);

        if (UtilityLibrary.IsValid(FallingRockToSpawn))
        {
            FallingObject SpawnedObject = FallingRockToSpawn.GetComponent<FallingObject>();
            SpawnedObject.ObjectIdentify(this, Scale);

            SpawnedObject.transform.position = AttackTarget.transform.position;
            SpawnedObject.transform.position += new Vector3(0, 5, 0); // 대상 머리 위에 떨어지도록

            FallingRockToSpawn.transform.localScale = new Vector3(4, 4, 4); // 기본 사이즈
            FallingRockToSpawn.transform.localScale *= Scale;
            FallingRockToSpawn.SetActive(true);
        }
    }
    #endregion

    void AttackPatternList()
    {
        if (Pattern_01)
        {
            Mino_SpecialAbility();

            // 행동 종료 후 대기 실행
            Wait(5);
        }

        else if (Pattern_02)
        {
            Mino_RushAbility();

            // 행동 종료 후 대기 실행
            Wait(3, 0.1f);
        }

        else if (Pattern_03)
        {
            Mino_FallingRock();

            // 행동 종료 후 대기 실행
            Wait(4, 0.1f);
        }

        else if (!Pattern_01 && !Pattern_02) // 만약 패턴1도 2도 쿨타임인데 할 수 있는 게 없을 때
        {
            // 우선 평타 시도
            if (Primary_Attack)
            {
                if (!InAttackRange)
                {
                    Debug.Log("Try Primary Attack! But Player is not In Range");
                    P1_CoolTimer += 10;
                    P2_CoolTimer += 5;
                    P3_CoolTimer += 10;

                    // 유저가 거리를 크게 벌리면 쿨다운 실행
                    Wait(1f);
                    return;
                }

                Mino_PrimaryAttack();

                Wait(2f);
                return;
            }

            // 평타도 쿨타임 중이라면 점프 이동 실행

            JumpMove_NoWait();

            if (!Primary_Attack)
            {
                Primary_Attack = true;
            }

            Wait(3);
        }

        
    }


    void RunBehaivior()
    {
        BossMinotaurAI();
    }

    void BossMinotaurAI()
    {
        if (!IsDead && !IsWaiting) // 대기중일 때도 아무것도 하면 안됨 (대기중인 경우에도 연산이 필요해질 경우 추후 처리)
        {
            // 모든 행동은 살아있을 때에만 하도록
            InAttackRange = TargetIsInRange();

            if (IsDamaged) // 피격 시에 해야할 일이 있다면 여기 기재
            {

            }

            else
            {
                #region Case: Idle
                if (UsingStateMachine.CurState == _IdleState)
                {
                    if (!AttackTarget) // 이후 공격 사거리등 조건 추가
                    {
                        // 타겟을 포착하지 못함

                        // 이후 배회를 추가하거나 플립을 통해서 탐색 기능 추가 고민
                    }

                    else if (AttackTarget)
                    {
                        AttackPatternList();
                    }
                }
                #endregion

                #region Case: Move
                else if (UsingStateMachine.CurState == _MoveState)
                {
                    if (AttackTarget)
                    {
                        // 이동중이면서 타겟을 잡은 경우
                        IsRunning = true;

                    }

                    else
                    {
                        // 만약 타겟을 놓친 경우
                        // 패트롤
                        IsRunning = false;
                    }
                }
                #endregion

                #region Case: Attack
                else if (UsingStateMachine.CurState == _MeleeAttackState)
                {

                }
                #endregion
            }
        }

        else // 죽었을 때
        {
            
        }
    }
}
