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
        // �̹� ������ �����ϰ� ��Ƽ���̷� �ٲٴ°Ŷ� ī��Ʈ -1
        MeleeHitBox_Controller.SetMeleeBoxSize(SwordHitBoxOffsetAndSize[1]);
    }
    public void Minotaur_SetAbilityRushRange()
    {
        MeleeHitBox_Controller.DamageRateIdx = 2;
        MeleeHitBox_Controller.SetMeleeBoxActivate(true);
        // �̹� ������ �����ϰ� ��Ƽ���̷� �ٲٴ°Ŷ� ī��Ʈ -1
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
                Debug.LogWarning("����׿� ���� AI ���� �ߴ�");
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
        // ���� ���� �ֻ��� �켱��
        // ����ġ�� ���� ��� ����
        Debug.Log("1 ����");
        // ��Ÿ�� ����
        StartCoolDown(ref P1_CoolTimer, ref Pattern_01);
        UsingStateMachine.ChangeState(_GroundAbility_01);

        // �ൿ ���� �� ��� ����
        Wait(3);
    }

    void Mino_RushAbility()
    {
        // ���� ���� �켱�� 2����
        // ���� ����
        Debug.Log("����!");
        // ��Ÿ�� ����
        StartCoolDown(ref P2_CoolTimer, ref Pattern_02);
        UsingStateMachine.ChangeState(_GroundAbility_02);
    }

    void Mino_PrimaryAttack()
    {
        // ���� ���� ������ �켱��
        // ������ �������ϸ� �ȵǴ� ���� ��Ÿ�� ����
        Debug.Log("��Ÿ ����");
        // �Ϲ� ���� ����
        UsingStateMachine.ChangeState(_MeleeAttackState);
        // ��Ÿ�� ����
        StartCoolDown(ref PA_CoolTimer, ref Primary_Attack);
    }

    void Mino_FallingRock()
    {
        // ���� ���� ������ �켱��
        // ������ �������ϸ� �ȵǴ� ���� ��Ÿ�� ����
        Debug.Log("���� ��ȯ");
        // ���� ����
        UsingStateMachine.ChangeState(_GroundAbility_03);
        // ��Ÿ�� ����
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
            SpawnedObject.transform.position += new Vector3(0, 5, 0); // ��� �Ӹ� ���� ����������

            FallingRockToSpawn.transform.localScale = new Vector3(4, 4, 4); // �⺻ ������
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

            // �ൿ ���� �� ��� ����
            Wait(5);
        }

        else if (Pattern_02)
        {
            Mino_RushAbility();

            // �ൿ ���� �� ��� ����
            Wait(3, 0.1f);
        }

        else if (Pattern_03)
        {
            Mino_FallingRock();

            // �ൿ ���� �� ��� ����
            Wait(4, 0.1f);
        }

        else if (!Pattern_01 && !Pattern_02) // ���� ����1�� 2�� ��Ÿ���ε� �� �� �ִ� �� ���� ��
        {
            // �켱 ��Ÿ �õ�
            if (Primary_Attack)
            {
                if (!InAttackRange)
                {
                    Debug.Log("Try Primary Attack! But Player is not In Range");
                    P1_CoolTimer += 10;
                    P2_CoolTimer += 5;
                    P3_CoolTimer += 10;

                    // ������ �Ÿ��� ũ�� ������ ��ٿ� ����
                    Wait(1f);
                    return;
                }

                Mino_PrimaryAttack();

                Wait(2f);
                return;
            }

            // ��Ÿ�� ��Ÿ�� ���̶�� ���� �̵� ����

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
        if (!IsDead && !IsWaiting) // ������� ���� �ƹ��͵� �ϸ� �ȵ� (������� ��쿡�� ������ �ʿ����� ��� ���� ó��)
        {
            // ��� �ൿ�� ������� ������ �ϵ���
            InAttackRange = TargetIsInRange();

            if (IsDamaged) // �ǰ� �ÿ� �ؾ��� ���� �ִٸ� ���� ����
            {

            }

            else
            {
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
                        AttackPatternList();
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

                    }

                    else
                    {
                        // ���� Ÿ���� ��ģ ���
                        // ��Ʈ��
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

        else // �׾��� ��
        {
            
        }
    }
}
