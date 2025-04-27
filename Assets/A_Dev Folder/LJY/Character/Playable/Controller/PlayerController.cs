using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using Project.Enums;
using Unity.VisualScripting;
using Unity.Behavior;

public class PlayerController : MonoBehaviour
{
    #region Ability Delegate

    #region Double Jump Changer
    public delegate void FOnItemConditionChanged();
    public FOnItemConditionChanged OnDoubleJumpItemChange;

    #region Callback
    void AdditiveJumpSet()
    {
        if (Player.PlayerSpec.CanDoubleJump)
        {
            ResetAdditiveJump();
        }
        else
        {
            UseAdditiveJump();
        }
    }
    #endregion

    #endregion

    #endregion

    PlayerBase Player;
    bool bCanActionControl = true;
    bool bMasterControl = true;

    public void EnableActionControl(bool Condition)
    {
        bCanActionControl = Condition;
    }

    public void EnableMasterControl(bool Condition)
    {
        bMasterControl = Condition;
    }

    // ToDo List
    /*
     ���� �� �ٸ� ��Ÿ ���� ����
    �ִϸ��̼� ������Ʈ �ӽ� ��� ����
     */

    private void Awake()
    {
        Player = GetComponent<PlayerBase>();
    }

    private void Start()
    {
        OnDoubleJumpItemChange += AdditiveJumpSet;
    }

    void Update()
    {
        if (Player.GroundDetect()) // ���� ���¿����� ����
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Player.IsRunning = true;
            }
            else
            {
                Player.IsRunning = false;
            }
        }

        PressedJumpKey = Input.GetKey(KeyCode.Space);
    }

    #region LJY - Actions

    #region Move
    public Vector2 InputVec { get; private set; }

    void OnMove(InputValue Value)
    {
        if (bCanActionControl && bMasterControl)
            InputVec = Value.Get<Vector2>();
    }

    #endregion

    #region Jump

    #region Down Jump
    IEnumerator PlatformIgnore()
    {
        CompositeCollider2D PlatformCollider = Player.CurPlatform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(Player.CharacterCollider, PlatformCollider);
        Player.AddTag(Player.Tag_DetectorDisable);

        yield return new WaitForSeconds(0.3f);
        Player.RemoveTag(Player.Tag_DetectorDisable);

        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(Player.CharacterCollider, PlatformCollider, false);
    }
    #endregion

    #region Double Jump
    public int AdditiveJumpCount { get; private set; } = 0;

    public void UseAdditiveJump()
    {
        AdditiveJumpCount = 0;
    }
    public void ResetAdditiveJump()
    {
        AdditiveJumpCount = 1;
    }
    #endregion

    public bool PressedJumpKey { get; private set; } = false;

    void OnJump()
    {
        if(bCanActionControl && bMasterControl)
        {
            if (Player.PlayerSpec.CanDoubleJump && (Player.UsingStateMachine.CurState == Player._JumpState || Player.UsingStateMachine.CurState == Player._AirborneState))
            {
                // ���� ������ �� �� �����鼭
                // ���ÿ� �������� ���¶��
                if (AdditiveJumpCount > 0)
                {
                    // ���� ���� ������ ���鼭 ���ÿ� �߰� ����  Ƚ���� 0���� ���� ��, ���ÿ� ���� ���� ���� ���¿�����
                    Player.SetDoubleJumpCondition(true);
                    UseAdditiveJump();
                    Player.UsingStateMachine.ChangeState(Player._JumpState);
                }
            }

            if (!Player.IsBusy)
            {
                if(Input.GetKey(KeyCode.S) && Player.CurPlatform)
                {
                    // �ϰ� ����
                    Player.UsingStateMachine.ChangeState(Player._AirborneState);
                    StartCoroutine(PlatformIgnore());
                    return;
                }

                if (!Input.GetKey(KeyCode.S) && (Player.UsingStateMachine.CurState == Player._IdleState || Player.UsingStateMachine.CurState == Player._MoveState) 
                    && Player.GroundDetect())
                {
                    // �÷��̾� ���°� ������̰ų� �̵����̸鼭, ���� ������ ��
                    Player.UsingStateMachine.ChangeState(Player._JumpState);
                }
            }
        }
    }
    #endregion

    #region Weapon Change
    void OnWeaponChange()
    {
        if(!Player.IsBusy && bCanActionControl &&  bMasterControl)
        {
            //Debug.Log("Weapon Change!");
            switch (Player.CommonSpec.GetCharacterName)
            {
                case "Unity Chan":
                    if (Player.CurWeapon == EWeaponState.Sword)
                        Player.CurWeapon = EWeaponState.Pistol;
                    else
                        Player.CurWeapon = EWeaponState.Sword;
                    break;

                default:
                    Debug.LogError("Weapon Change Target Character Name Error");
                    break;
            }

            Player.PlayerUI.OnWeaponChanged();
        }
    }
    #endregion

    #region Attack
    void OnAttack()
    {
        if(bMasterControl && bCanActionControl)
        {
            if(!Player.IsBusy && Player.GroundDetect()) // ���� ������ �����Ƿ� ���� ���¿����� ���. �߰��� ����
            {
                if (InputVec != Vector2.zero)
                    InputVec = Vector2.zero;

                switch (Player.CurWeapon)
                {
                    case EWeaponState.Sword:
                        Player.UsingStateMachine.ChangeState(Player._MeleeAttackState);
                        break;

                    case EWeaponState.Pistol:
                        Player.UsingStateMachine.ChangeState(Player._RangeAttackState);
                        break;

                    default:
                        Debug.LogError("Please Set Playable Character Weapon State");
                        break;
                }
            }
        }
    }
    #endregion

    #region Dodge
    void OnDodge()
    {
        if(bCanActionControl && bMasterControl)
        {
            // Debug.Log("Dodge");
            if ((Player.UsingStateMachine.CurState == Player._IdleState || Player.UsingStateMachine.CurState == Player._MoveState) && Player.bCanDodge)
            {
                // �÷��̾� ���°� ����� / �̵��� / ������ ��� && ���� ���� ������ ��
                StartCoroutine(CoolDown("Dodge"));

                Player.UsingStateMachine.ChangeState(Player._DodgeState);
                //OnDodgeDelegate();
            }
        }
    }
    #endregion

    #region Quick Slots

    void UsePotion(float HealRate, EHealType PotionType)
    {
        float HealAmount = 0;

        if (PotionType == EHealType.HPType)
        {
            int CurPotion = Player.PlayerSpec.GetHasHPPotion;
            if (CurPotion <= 0)
            {
                Debug.Log("�������� ü�� ���� ����");
                return;
            }

            HealAmount = Player.PlayerSpec.GetCurHP * 0.01f; // ü�� ���

            Player.Search_Effect(EEffectType.HP_Recovery);
            Player.PlaySoundSelect(Player.PlayerUI.HP_Potion_Sound, EVolumeType.Effect);
        }
        else if (PotionType == EHealType.MPType)
        {
            int CurPotion = Player.PlayerSpec.GetHasMPPotion;
            if (CurPotion <= 0)
            {
                Debug.Log("�������� ���� ���� ����");
                return;
            }

            HealAmount = Player.PlayerSpec.GetCurMP * 0.01f; // ���� ���

            Player.Search_Effect(EEffectType.MP_Recovery);
            Player.PlaySoundSelect(Player.PlayerUI.MP_Potion_Sound, EVolumeType.Effect);
        }

        if(HealRate > 0)
        {
            HealAmount *= HealRate;
        }
        else
        {
            // ȸ�� ������ 0 ���϶��
            Debug.LogWarning("���� ȸ�� ������ �缳�� ���ּ���" + " Heal Rate: " + HealRate);
        }

        Player.PlayerSpec.AddPotion(-1, PotionType);
        UtilityLibrary.TakeHealToSelf(Player, HealAmount, PotionType);
    }

    void OnUseQuickSlot01()
    {
        // ���� �����ۿ� �� �� �������� �߰�
        //Debug.Log("Slot 01");
        UsePotion(50, EHealType.HPType);
    }

    void OnUseQuickSlot02()
    {
        //Debug.Log("Slot 02");
        UsePotion(50, EHealType.MPType);
    }
    #endregion

    #region Interaction
    [HideInInspector] public bool IsInteraction = false;
    [HideInInspector] public Interactable_Object InteractTarget;

    void OnInteraction() // f
    {
        // �� ���� ��ȣ�ۿ��� �� �ִ� ����� 1�� ��ü
        if(!IsInteraction)
        {
            // ���� ��ȣ�ۿ����� �ƴ� ���� ����
            if(InteractTarget)
            {
                InteractTarget.InteractEffect();
            }
        }
    }

    #endregion

    #region Skill
    void OnSkill()
    {
        if (bCanActionControl && bMasterControl)
        {
            Debug.Log("Skill");

            switch(Player.CurWeapon)
            {
                case EWeaponState.Sword:
                    if(Player.Ability01Flag)
                    {
                        Player.UseAbility01();
                        Player.UsingStateMachine.ChangeState(Player._GroundAbility_01);
                    }
                    break;

                case EWeaponState.Pistol:
                    if (Player.Ability02Flag)
                    {
                        Player.UseAbility02();
                        Player.UsingStateMachine.ChangeState(Player._GroundAbility_02);
                    }
                    break;
            }
        }
    }
    #endregion

    #endregion

    #region KHT 

    void OnInventory() // i
    {
        if (!bMasterControl)
            return;

        if (UIManager.Instance == null)
            return;

        UIManager.Instance.ToggleInventory();
    }

    void OnMap() // m
    {
        Debug.Log("mmmmmmmm");
    }

    void OnPause() // esc
    {
        Debug.Log("esc");
    }
    #endregion

    #region Utility

    IEnumerator CoolDown(string AbilityName)
    {
        float CoolTime = 0;

        switch (AbilityName)
        {
            case "Dodge":
                Player.SetDodgeBoolean(false);
                Player.PlayerUI.OnDodgeStart();
                CoolTime = Player.AbilitySpec.GetDodgeCoolTime;
                //Debug.Log(CoolTime);
                break;

            default:
                Debug.Log("Name Error");
                break;
        }

        yield return new WaitForSecondsRealtime(CoolTime);

        switch (AbilityName)
        {
            case "Dodge":
                Player.SetDodgeBoolean(true);
                break;
        }
    }

    #endregion


}
