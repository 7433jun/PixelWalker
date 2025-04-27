using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Project.Enums;

public class PlayerBase : CharacterBase
{
    public PlayerController PCont { get; private set; }
    public PlayerUIComponent PlayerUI { get; private set; }
    public PlayerSpecData PlayerSpec { get; private set; }
    
    [HideInInspector] public bool IsTurn = false;

    #region Hud Value

    #region Skill Ability Timer
    public bool Ability01Flag { get; private set; } = true;
    public bool Ability02Flag { get; private set; } = true;

    public void UseAbility01()
    {
        Ability01FlagControl(false);
        Ability_01_Timer = PlayerSpec.GetAbility_01_CoolDown;
    }

    public void UseAbility02()
    {
        Ability02FlagControl(false);
        Ability_02_Timer = PlayerSpec.GetAbility_02_CoolDown;
    }

    public void Ability01FlagControl(bool Condition)
    {
        Ability01Flag = Condition;
    }

    public void Ability02FlagControl(bool Condition)
    {
        Ability02Flag = Condition;
    }

    public float Ability_01_Timer {  get; private set; }
    public float Ability_02_Timer {  get; private set; }

    public float GetAbility01TimerProgress()
    {
        return Ability_01_Timer / PlayerSpec.GetAbility_01_CoolDown;
    }

    public float GetAbility02TimerProgress()
    {
        return Ability_02_Timer / PlayerSpec.GetAbility_02_CoolDown;
    }

    void AbilityTimerCaculator()
    {
        if (Ability_01_Timer > 0)
        {
            Mathf.Clamp(Ability_01_Timer -= Time.deltaTime, 0, PlayerSpec.GetAbility_01_CoolDown);
        }
        else
        {
            Ability01FlagControl(true);
        }

        if (Ability_02_Timer > 0)
        {
            Mathf.Clamp(Ability_02_Timer -= Time.deltaTime, 0, PlayerSpec.GetAbility_02_CoolDown);
        }
        else
        {
            Ability02FlagControl(true);
        }
    }

    #endregion

    #endregion

    #region Interface
    public override UIComponentBase GetCharacterUIComponent()
    {
        return PlayerUI;
    }

    #endregion

    #region Buff Skill
    public void StartBuff_01()
    {
        StartCoroutine(BuffPackage01(5));
    }

    protected virtual IEnumerator BuffPackage01(float Duration)
    {
        // 원하는 버프 패키지 작성
        yield return null;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        PCont = GetComponentInChildren<PlayerController>();
        PlayerUI = GetComponentInChildren<PlayerUIComponent>();
        PlayerSpec = CommonSpec as PlayerSpecData;

    }

    protected override void Start()
    {
        base.Start();


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void Update()
    {
        base.Update();

        PlayerDashLogic();

        RecoveryNonBattleState();

        AbilityTimerCaculator();

        EventDebugger();
    }

    void EventDebugger()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //PlayerSpec.EnableRegenHP(true);
            AfterImageEffector.StartAfterImageEffect();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //PlayerSpec.EnableRegenHP(false);
        }
    }

    #region Special Ability Item Effect

    #region Regen HP
    public bool bIsBattle { get; private set; } = false;
    float RegenPeriod = 3;

    #region Event Handler
    public Coroutine RegenEventHandle { get; private set; } = null;

    float RecoveryBattleStateToNonBattle = 4; // 전투 상태 진입 하고 다시 비전투로 돌아오는 시간
    float RecoveryStateTimer = 0;

    void RecoveryNonBattleState()
    {
        // 리젠 이벤트가 유효한 경우의 처리
        if (bIsBattle)
        {
            // 전투 상태였다면
            if (RecoveryStateTimer < RecoveryBattleStateToNonBattle)
            {
                RecoveryStateTimer += Time.deltaTime;
            }
            else
            {
                InBattleState(false);
            }
        }
    }
    #endregion

    #region Coroutine Handler
    public void InBattleState(bool Condition) // true의 조건: 공격하거나 피격받으면 전투 상태로 취급
    {
        if (bIsBattle)
        {
            // 이미 전투중인데 또 전투중이라 호출될 경우
            RecoveryStateTimer = 0;
        }

        bIsBattle = Condition;
    }

    public Coroutine StartRegenHP()
    {
        return StartCoroutine(RegenHP());
    }
    #endregion

    #region Regen Coroutine
    IEnumerator RegenHP()
    {
        yield return new WaitForSecondsRealtime(RegenPeriod);

        while (PlayerSpec.CanRegenHP)
        {
            //Debug.Log("Regen Start");
            // 이벤트가 지속되는 동안엔 무한하게 반복
            // 전투중일 땐 아무것도 하지 않음
            // 풀피면 아무것도 하지 않음
            if (!bIsBattle && PlayerSpec.GetCurHP < PlayerSpec.GetMaxHP)
            {
                //Debug.Log(bIsBattle);
                // 비전투 중이면서 체력이 풀피도 아닌 경우
                PlayerSpec.HPRecovery();
                yield return new WaitForSecondsRealtime(RegenPeriod);
            }

            else
            {
                yield return new WaitForSecondsRealtime(1);
            }
        }

        //Debug.Log("Regen End");
        if(PlayerSpec.RegenHpHandle != null)
        {
            //Debug.Log("RegenHpHandle Reset");
            PlayerSpec.RegenHpHandle = null;
        }
        yield return null;
    }
    #endregion

    #endregion

    #endregion

    public Vector2 GetMoveVec() => MoveVec;

    public void HandleNormalMovement()
    {
        if (!IsRunning)
            MoveVec = PCont.InputVec * CommonSpec.GetWalkSpeed;
        else
            MoveVec = PCont.InputVec * CommonSpec.GetDashSpeed;

        SetVelocity_XAxis_WithFlip(MoveVec);
    }
    public void HandleTurningMovement(float LastMoveVectorX)
    {
        SetVelocity_Just_XAxis(LastMoveVectorX);
    }

    public void PlayerDashLogic()
    {
        Anim.SetBool("Dash", IsRunning);
    }

    #region Player Notify
    public void MoveControl()
    {
        IsTurn = false;
        Anim.SetBool("Turn", false);
        Anim.SetBool("Dash Break", false);
    }
    #endregion
}
