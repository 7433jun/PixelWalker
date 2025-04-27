using System.Collections;
using UnityEngine;

public class BossBase : EnemyBase
{
    #region AI Service
    public void GetPlayer()
    {
        GameObject FoundData = GameObject.FindWithTag("Player");
        AttackTarget = FoundData.GetComponent<CharacterBase>();
    }
    #endregion

    #region Boss Generic

    #region Appear
    public void AppearBoss()
    {
        GetPlayer();
        Anim.SetTrigger("Intro Start");
        Anim.SetBool("NonBattle", false);
    }
    #endregion

    #region Notify

    protected bool StartBattle;
    public void AppearEnd()
    {
        Anim.SetTrigger("Battle Start");
        StartBattle = true;
    }
    #endregion

    #endregion

    #region Jump Move Point
    [SerializeField] Transform[] MovePoint;
    Transform CurMovePoint;
    public Vector2 NewMove { get; private set; }

    public Vector2 GetMovePoint()
    {
        int ArraySize = MovePoint.Length;
        Transform NewMovePoint = MovePoint[Random.Range(0, ArraySize)];

        if (CurMovePoint == null)
        {
            // �ѹ��� �̵� ��������
            CurMovePoint = NewMovePoint;
        }
        else
        {
            // �̹� �̵��� ����Ʈ�� �ִٸ�
            // �̵�ĭ�� �ϳ����̸� �����ؾ���
            if (ArraySize != 1)
            {
                if(CurMovePoint == NewMovePoint)
                {
                    while (CurMovePoint == NewMovePoint)
                    {
                        // ����ĭ�� ���ο� �̵� ������ �޶��� ������ ���� ����
                        NewMovePoint = MovePoint[Random.Range(0, ArraySize)];
                        //bool TempBool = CurMovePoint != NewMovePoint;
                        //Debug.Log("ReRoll! Cur Point: " + CurMovePoint + " / New Point: " + NewMovePoint + " | Break: " + TempBool);
                    }
                }

                // �ߺ��̾��� �ƴϾ��� �ϴ� ���� ����Ǵ� �������� �� ���� �ʼ�
                CurMovePoint = NewMovePoint;
            }
        }
        return CurMovePoint.position;
    }
    #endregion

    #region Task

    #region Task Flag
    [SerializeField] protected bool Primary_Attack = true;
    [SerializeField] protected bool Pattern_01 = true;
    [SerializeField] protected bool Pattern_02 = true;
    [SerializeField] protected bool Pattern_03 = true;

    [SerializeField] protected float PrimaryAttackCoolDown = 0;
    protected float PA_CoolTimer = 0;

    [SerializeField] protected float Pattern_01CoolDown = 0;
    [SerializeField] protected float P1_CoolTimer = 0;

    [SerializeField] protected float Pattern_02CoolDown = 0;
    [SerializeField] protected float P2_CoolTimer = 0;

    [SerializeField] protected float Pattern_03CoolDown = 0;
    [SerializeField] protected float P3_CoolTimer = 0;
    #endregion

    #region Jump Move
    protected bool JumpMoving()
    {
        if (UsingStateMachine.CurState == _IdleState)
        {
            NewMove = GetMovePoint();
            //Debug.Log("Move Point: " + NewMove);
            //Debug.Log("Cur Move Point: " + CurMovePoint.position);
            return true;
        }
        return false;
    }

    public void JumpMove_withWait()
    {
        if(JumpMoving())
        {
            // ���� �ൿ���� �ű�� �� ������ ��� ó�� ����
            UsingStateMachine.ChangeState(_JumpState);
        }
        // ���� �̵� ���� ���� [v]
        // ��Ÿ�� ���� 
        // ���� ������ �� ���� �� �� ���� ��� �̿� [v]
        // �� ȿ�� ����� �Ϲ� ���� ��Ÿ�� �ʱ�ȭ

        // �ൿ ���� �� ��� ����
        Wait(3);
    }

    public void JumpMove_NoWait()
    {
        if (JumpMoving())
        {
            // ���� �ൿ���� �ű�� �� ������ ��� ó�� ����
            UsingStateMachine.ChangeState(_JumpState);
        }
    }

    #endregion

    #region Wait Node
    protected void Wait(float WaitTime, float Deviation = 0)
    {
        if(!IsWaiting)
        {
            float FinalWaitTime = WaitTime;

            if (Deviation > 0)
            {
                // ���� ���� 0�� �ƴ� ��� (Ư�� ���� ���� ���� ���)
                float DeviationValue = Random.Range(0.1f, Deviation);
                
                if(UtilityLibrary.RandomBool())
                {
                    DeviationValue *= -1;
                }

                FinalWaitTime += DeviationValue;
            }
            else if (Deviation < 0)
            {
                Debug.LogWarning("Waiting Node Factor Value Transfer Error");
            }

            StartCoroutine(WaitRule(FinalWaitTime));
        }
        else
        {
            Debug.Log("Wait Now");
        }
    }

    IEnumerator WaitRule(float WaitTime)
    {
        IsWaiting = true;
        //Debug.Log("Start Wait!");

        yield return new WaitForSecondsRealtime(WaitTime);

        IsWaiting = false;
        //Debug.Log("WaitEnd");
    }
    #endregion

    #region Generic Pattern CoolDown
    void CoolDownManager()
    {
        if(!Primary_Attack)
        {
            CoolDownRule(ref PA_CoolTimer, PrimaryAttackCoolDown, ref Primary_Attack);
        }
        if (!Pattern_01)
        {
            CoolDownRule(ref P1_CoolTimer, Pattern_01CoolDown, ref Pattern_01);
        }
        if (!Pattern_02)
        {
            CoolDownRule(ref P2_CoolTimer, Pattern_02CoolDown, ref Pattern_02);
        }
        if (!Pattern_03)
        {
            CoolDownRule(ref P3_CoolTimer, Pattern_03CoolDown, ref Pattern_03);
        }
    }

    protected void StartCoolDown(ref float Timer, ref bool AbilityFlag)
    {
        Timer = 0;
        AbilityFlag = false;
    }

    void CoolDownRule(ref float Timer, float AbilityCoolTime, ref bool AbilityFlag)
    {
        if (Timer < AbilityCoolTime)
        {
            Timer += Time.deltaTime;
        }

        else
        {
            AbilityFlag = true;
        }
    }

    IEnumerator CoolDown(string PatternName)
    {
        switch(PatternName)
        {
            case "Primary Attack":

                break;

            case "Pattern01":
                Pattern_01 = false;
                yield return new WaitForSecondsRealtime(PrimaryAttackCoolDown);
                if(Pattern_01)
                {
                    yield return null;
                }

                break;

            case "Pattern02":

                break;

            case "Pattern03":

                break;

            default:
                Debug.LogWarning("Boss Pattern CoolDown Error");
                yield return null;
                break;
        }
    }

    #endregion

    #endregion

    #region Boss Additive Blackboard
    [Header("[Debug] Boss Additive Blackboard")]
    [SerializeField] protected bool IsWaiting = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        AddTag(Tag_Boss);
        AddTag(Tag_SuperArmor); // ������ �⺻������ ��� ���۾Ƹ� ��� ����
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Start()
    {
        base.Start();
        Anim.SetBool("NonBattle", true);
    }

    protected override void Update()
    {
        base.Update();
        CoolDownManager();
    }


}
