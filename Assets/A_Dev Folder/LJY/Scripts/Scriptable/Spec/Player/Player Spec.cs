using System.Collections;
using UnityEngine;
using Project.Enums;

[CreateAssetMenu(fileName = "Character Spec Data", menuName = "Scriptable Object/Player Spec", order = int.MinValue)]
public class PlayerSpecData : CharacterSpecData
{
    [Header("Player Inventory")]
    // ======================================================= //
    [SerializeField] int Gold;
    public int GetGold { get { return Gold; } }
    // ======================================================= //
    [SerializeField] int HasHPPotion;
    public int GetHasHPPotion { get { return HasHPPotion; } }
    // ======================================================= //
    [SerializeField] int HasMPPotion;
    public int GetHasMPPotion { get { return HasMPPotion; } }
    // ======================================================= //
    [Header("Player Spec")]
    [SerializeField] float Ability_01_CoolDown;
    public float GetAbility_01_CoolDown => Ability_01_CoolDown;
    // ======================================================= //
    [SerializeField] float Ability_02_CoolDown;
    public float GetAbility_02_CoolDown => Ability_02_CoolDown;


    #region Player Voice List

    #endregion

    #region Use Item
    public void AddPotion(int AdditiveAmount, EHealType PotionType)
    {
        if(PotionType == EHealType.HPType)
        {
            if(HasHPPotion <= 0 && AdditiveAmount < 0)
            {
                Debug.LogWarning("사용 가능한 포션 수 부족");
                return;
            }
            else if (HasHPPotion >= 99 && AdditiveAmount > 0)
            {
                Debug.LogWarning("포션 수 최대");
                return;
            }

            HasHPPotion += AdditiveAmount;
            //Debug.Log("Get HP Potion! " + HasHPPotion);
            HasHPPotion = Mathf.Clamp(HasHPPotion, 0, 99);
        }
        else if(PotionType == EHealType.MPType)
        {
            if (HasMPPotion <= 0 && AdditiveAmount < 0)
            {
                Debug.LogWarning("사용 가능한 포션 수 부족");
                return;
            }
            else if (HasMPPotion >= 99 && AdditiveAmount > 0)
            {
                Debug.LogWarning("포션 수 최대");
                return;
            }

            HasMPPotion += AdditiveAmount;
            Debug.Log("Get MP Potion! " + HasMPPotion);
            //HasMPPotion = Mathf.Clamp(HasMPPotion, 0, 99);
        }
        GetPlayerCharacter().PlayerUI.OnItemNumChanged();
    }

    public bool AddGold(int AdditiveAmount)
    {
        if(AdditiveAmount < 0)
        {
            // 더하려는 값이 음수일 때 (사용)
            if(Gold < AdditiveAmount)
            {
                Debug.LogWarning("Not Enough Gold");
                return false;
            }
        }

        // 자산을 추가할 경우는 별도의 검증을 거치지 않음
        Gold += AdditiveAmount;
        GetPlayerCharacter().PlayerUI.OnGoldChanged();
        return true;
    }
    #endregion

    #region Item Special Ability

    #region Equip Special Ability Item List
    [Header("[Debug] Equip Item Condition")]
    public bool CanRegenHP = false;

    public bool CanDoubleJump = false;

    #endregion

    #region HP Regen
    public Coroutine RegenHpHandle;

    public void EnableRegenHP(bool Condition)
    {
        CanRegenHP = Condition;

        if(CanRegenHP)
        {
            if(RegenHpHandle == null)
            {
                RegenHpHandle = GetPlayerCharacter().StartRegenHP();
            }
            else
            {
                Debug.Log("Already Heal");
            }
        }
    }

    public void HPRecovery()
    {
        if(CurHP < GetMaxHP)
        {
            float HealAmount = GetMaxHP * 0.01f;
            HealAmount *= RecoveryRate;

            //GetPlayerCharacter().PlaySoundSelect(); 굳이 리젠에는 안넣어도 될 듯?
            GetPlayerCharacter().Search_Effect(EEffectType.HP_Regen);

            CurHP += HealAmount;
            // 1% 만큼만 회복
            CurHP = Mathf.Clamp(CurHP, 0, GetMaxHP);
            StatusChanged();
        }
    }
    #endregion

    #region Double Jump
    public void EnableDoubleJump(bool Condition)
    {
        CanDoubleJump = Condition;
        GetPlayerCharacter().PCont.OnDoubleJumpItemChange();
    }
    // 더블 점프의 능력 자체는 플레이어 컨트롤러에서
    #endregion

    #endregion

    #region Utility
    PlayerBase GetPlayerCharacter()
    {
        return Owner as PlayerBase;
    }
    #endregion
}
