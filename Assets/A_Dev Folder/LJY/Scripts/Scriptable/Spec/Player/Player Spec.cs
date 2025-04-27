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
                Debug.LogWarning("��� ������ ���� �� ����");
                return;
            }
            else if (HasHPPotion >= 99 && AdditiveAmount > 0)
            {
                Debug.LogWarning("���� �� �ִ�");
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
                Debug.LogWarning("��� ������ ���� �� ����");
                return;
            }
            else if (HasMPPotion >= 99 && AdditiveAmount > 0)
            {
                Debug.LogWarning("���� �� �ִ�");
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
            // ���Ϸ��� ���� ������ �� (���)
            if(Gold < AdditiveAmount)
            {
                Debug.LogWarning("Not Enough Gold");
                return false;
            }
        }

        // �ڻ��� �߰��� ���� ������ ������ ��ġ�� ����
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

            //GetPlayerCharacter().PlaySoundSelect(); ���� �������� �ȳ־ �� ��?
            GetPlayerCharacter().Search_Effect(EEffectType.HP_Regen);

            CurHP += HealAmount;
            // 1% ��ŭ�� ȸ��
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
    // ���� ������ �ɷ� ��ü�� �÷��̾� ��Ʈ�ѷ�����
    #endregion

    #endregion

    #region Utility
    PlayerBase GetPlayerCharacter()
    {
        return Owner as PlayerBase;
    }
    #endregion
}
