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
                // ���ƴٰ� �ٽ� ã�Ҵٸ� ��ģ �ð� ����
            }

            return HitResult.collider.GetComponent<CharacterBase>();
        }

        if (AttackTarget && !HitResult)
        {
            // �̹� Ÿ���� ��Ҵ� �����ε� ����� �Ⱥ��̸�?
            if (MissingTime < RememberTargetTime)
            {
                MissingTime += Time.deltaTime;

                return AttackTarget; // ������ ������� Ÿ���� ��ȯ
            }

            else
            {
                // Ÿ���� ��ġ�� �ʹ� ���� �ð��� �귯������
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
            // ��� �ൿ�� ������� ������ �ϵ���

            if (IsDamaged) // �ǰ� �ÿ� �ؾ��� ���� �ִٸ� ���� ����
            {
                
            }

            else
            {
                #region Generic - Target Search Service & Get Distance this <-> Target
                /* All State Generic */
                AttackTarget = SearchTarget();

                if (AttackTarget)
                {
                    // Ÿ���� ���� ���� ���
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
                    if (!AttackTarget) // ���� ���� ��Ÿ��� ���� �߰�
                    {
                        // Ÿ���� �������� ����

                        // ���� ��ȸ�� �߰��ϰų� �ø��� ���ؼ� Ž�� ��� �߰� ���
                    }

                    else if (AttackTarget)
                    {
                        if (InAttackRange)
                        {
                            // ������ �������ϸ� �ȵǴ� ���� ��Ÿ�� ����

                            return;
                        }

                        // MoveState�� ����� �߰��� (�ʿ��� ��� �̵� ������ �̸��� �� ���� ���� ��)
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
                        // �̵����̸鼭 Ÿ���� ���� ���
                        IsRunning = true;

                        if(TargetIsBack())
                        {
                            Flip();
                        }


                        if (WallDetect())
                        {
                            // ĳ���Ͱ� Ÿ�� �߰� �� ��(��)�� �߰��ߴٸ� ���� && �ϴ� ������ ���� �پ��־�� ��
                            if(IsNotWisp())
                            {
                                UsingStateMachine.ChangeState(_JumpState);
                            }
                        }
                    }

                    else
                    {
                        // ���� Ÿ���� ��ģ ���
                        // ��Ʈ��
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

        else // �׾��� ��
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
        // �̸��� ����/����/�/���̾� ������ �ƴϸ� Ʈ��
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
