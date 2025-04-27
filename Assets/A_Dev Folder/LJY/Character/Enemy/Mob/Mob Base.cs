using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MobBase : EnemyBase
{
    public MobUIComponent EnemyUI { get; private set; }

    public ForwardTopographyDetector ForwardDetector { get; private set; }

    [Header("UI Offset")]
    public Vector2 MobHPSliderOffset;

    #region Interface
    public override UIComponentBase GetCharacterUIComponent()
    {
        return EnemyUI;
    }
    #endregion

    #region AI System

    #region Perception
    [Header("AI Sight")]
    [SerializeField] Vector2 BoxSize;
    [SerializeField] Vector2 BoxOffset;
    float DetectRange;
    LayerMask DetectTargetLayer;

    private void OnDrawGizmos()
    {
        if (bUseDrawDebug)
        {
            DetectRange = BoxSize.x;

            Vector2 Origin = (Vector2)transform.position + BoxOffset;
            Vector2 Dir = new Vector2(FacingDir, 0);

            Vector2 BoxCenter = Origin + Dir * (DetectRange / 2);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(BoxCenter, BoxSize);
        }
    }
    #endregion

    #region AbilityList
    string PrimaryAttack = "Primary Attack";
    bool CanPrimaryAttack = true;
    #endregion

    #region AI Service
    CharacterBase SearchTarget()
    {
        Vector2 Origin = (Vector2)transform.position + BoxOffset;
        Vector2 Dir = new Vector2(FacingDir, 0);

        Vector2 BoxCenter = Origin + Dir * (DetectRange / 2);

        RaycastHit2D HitResult = Physics2D.BoxCast(BoxCenter, BoxSize, 0, new Vector2(FacingDir, 0), DetectRange, DetectTargetLayer);

        if (HitResult && HitResult.collider.gameObject.CompareTag("Player"))
        {
            if (MissingTime > 0)
            {
                MissingTime = 0;
                // 놓쳤다가 다시 찾았다면 놓친 시간 리셋
            }

            return HitResult.collider.GetComponent<CharacterBase>();
        }

        if (AttackTarget && !HitResult)
        {
            // 이미 타겟을 잡았던 상태인데 현재는 안보이면?
            if (MissingTime < RememberTargetTime)
            {
                MissingTime += Time.deltaTime;

                return AttackTarget; // 아직은 기억중인 타겟을 반환
            }

            else
            {
                // 타겟을 놓치고 너무 많은 시간이 흘러버리면
                MissingTime = 0;
                UsingStateMachine.ChangeState(_IdleState);
            }
        }

        return null;
    }

    public bool CanChaseToTarget() => ForwardDetector.SearchedForwardGround();

    public void ClearBlackboard_Target()
    {
        AttackTarget = null;
    }

    #endregion

    #region AI Type
    protected void GenericMobAI()
    {
        if (!IsDead)
        {
            // 모든 행동은 살아있을 때에만 하도록

            if (IsDamaged) // 피격 시에 해야할 일이 있다면 여기 기재
            {
                
            }

            else
            {
                #region Generic - Target Search Service & Get Distance this <-> Target
                /* All State Generic */
                AttackTarget = SearchTarget();

                if (AttackTarget)
                {
                    // 타겟을 포착 중인 경우
                    InAttackRange = TargetIsInRange();
                    LastLookPoint = AttackTarget.transform.position;

                    if (TargetIsBack())
                    {
                        if (!IsBusy)
                            Flip();
                    }
                }

                if (InAttackRange == true && (UsingStateMachine.CurState == _IdleState || UsingStateMachine.CurState == _MoveState) && CanPrimaryAttack)
                {
                    UsingStateMachine.ChangeState(_MeleeAttackState);
                }
                /* All State Generic */
                #endregion

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
                        if (InAttackRange)
                        {
                            // 공격을 무한정하면 안되니 이후 쿨타임 적용

                            return;
                        }

                        // MoveState로 대상을 추격함 (필요할 경우 이동 로직을 이리로 뺄 수도 있을 듯)
                        if (CanChaseToTarget())
                            UsingStateMachine.ChangeState(_MoveState);
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

                        if(TargetIsBack())
                        {
                            Flip();
                        }


                        if (WallDetect())
                        {
                            // 캐릭터가 타겟 추격 중 벽(땅)을 발견했다면 점프 && 일단 본인이 땅에 붙어있어야 함
                            if(IsNotWisp())
                            {
                                UsingStateMachine.ChangeState(_JumpState);
                            }
                        }
                    }

                    else
                    {
                        // 만약 타겟을 놓친 경우
                        // 패트롤
                        IsRunning = false;

                        if(CanChaseToTarget())
                            UtilityLibrary.MoveTo(this, LastLookPoint, 0, CommonSpec.GetWalkSpeed);
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
            if (!EnemyUI.SpawnedHPBar.IsDestroyed())
            {
                EnemyUI.OnMobDead();
            }
        }
    }
    #endregion

    #endregion

    bool IsNotWisp()
    {
        // 이름이 윈드/워터/어스/파이어 위습이 아니면 트루
        return EnemySpec.GetCharacterName != "Wind Wisp" && EnemySpec.GetCharacterName != "Water Wisp" && EnemySpec.GetCharacterName != "Earth Wisp" && EnemySpec.GetCharacterName != "Fire Wisp";
    }

    protected override void Awake()
    {
        base.Awake();
        EnemyUI = GetComponentInChildren<MobUIComponent>();
        DetectRange = BoxSize.x;
        EnemyUI.CreateWidget_Slider();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Start()
    {
        base.Start();
        DetectTargetLayer = LayerMask.GetMask("Player");
        ForwardDetector = GetComponentInChildren<ForwardTopographyDetector>();
        GetCharacterUIComponent().OnInitialized();
    }

    protected override void Update()
    {
        base.Update();
    }

    #region Enemy Anim Notify
    public void Enemy_PrimaryAttackEnd(float AttackCoolDown)
    {
        UsingStateMachine.ChangeState(_IdleState);

        StartCoroutine(Enemy_CoolDown(PrimaryAttack, AttackCoolDown));
    }
    #endregion

    #region Task Cool Down
    IEnumerator Enemy_CoolDown(string AbilityName, float CoolTime)
    {
        switch (AbilityName)
        {
            case "Primary Attack":
                CanPrimaryAttack = false;
                break;

        }

        yield return new WaitForSeconds(CoolTime);

        switch (AbilityName)
        {
            case "Primary Attack":
                CanPrimaryAttack = true;
                break;

        }
    }
    #endregion
}
