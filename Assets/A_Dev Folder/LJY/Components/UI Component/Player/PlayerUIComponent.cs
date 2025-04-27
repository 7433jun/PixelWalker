using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Project.Enums;

public class PlayerUIComponent : UIComponentBase
{
    #region OnItemNumChanged
    public delegate void FOnItemNumChanged();
    public delegate void FOnStateChanged();
    #region Item Num Change
    public FOnItemNumChanged OnItemNumChanged;

    public FOnItemNumChanged OnGoldChanged;
    #endregion

    #region Weapon Change
    public FOnStateChanged OnWeaponChanged;
    #endregion

    #region Dodge
    public FOnPercentChanged OnDodgeStart;
    #endregion

    #endregion

    [Header("UI Set")]
    [SerializeField] GameObject PlayerUISet;

    [Header("Player UI")]
    [SerializeField] Slider MP_ProgressBar;
    [SerializeField] TextMeshProUGUI HaveMoney;
    [SerializeField] TextMeshProUGUI HaveHPPotion;
    [SerializeField] TextMeshProUGUI HaveMPPotion;
    [SerializeField] Slider Dodge_ProgressBar;
    [SerializeField] Image CurWeaponType;
    [SerializeField] Image Skill_Image;
    [SerializeField] Image Skill_CoolDownCircle;
    [SerializeField] TextMeshProUGUI SkillCoolTime;

    [Header("Weapon Type")]
    [SerializeField] AudioClip WeaponChangeSound;
    [SerializeField] Sprite MeleeTypeIcon;
    [SerializeField] Sprite RangeTypeIcon;

    [Header("Skill")]
    [SerializeField] Sprite MeleeTypeSkill_Icon;
    [SerializeField] Sprite RangeTypeSkill_Icon;

    [Header("Potion")]
    public AudioClip HP_Potion_Sound;
    public AudioClip MP_Potion_Sound;

    #region Dodge Progress
    float DodgeTimer = 0; // ���α׷��� ������� ���� �߰� ����
    bool DodgeCaculateTrigger = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        // ���� ��� �ʱ�ȭ ��Ҹ� ���̺꿡�� �޾ƿ��� �۾� �ʿ�
        OnInitialized += SetHPRate;
        OnStatusChanged += SetHPRate;

        OnInitialized += SetMPRate;
        OnInitialized += SetMoneyText;
        OnInitialized += SetHPPotionText;
        OnInitialized += SetMPPotionText;
        OnInitialized += InitDodge;
        OnInitialized += WeaponIconChangeToStart;

        OnStatusChanged += SetMPRate;

        OnItemNumChanged += SetHPPotionText;
        OnItemNumChanged += SetMPPotionText;

        OnGoldChanged += SetMoneyText;

        OnDodgeStart += DodgeStart;

        OnWeaponChanged += WeaponIconChange;
    }

    void Start()
    {
        if(!GameObject.Find("HUD Canvas"))
        {
            Instantiate(PlayerUISet);
        }

        GameObject HPProgressBar = GameObject.Find("HP Slider");
        GameObject MPProgressBar = GameObject.Find("MP Slider");
        GameObject Gold = GameObject.Find("Up Right");
        GameObject HPPotion = GameObject.Find("HP Count Text");
        GameObject MPPotion = GameObject.Find("MP Count Text");
        GameObject DodgeProgressBar = GameObject.Find("Dash Slider");
        GameObject CurrentWeapon = GameObject.Find("Weapon Image");
        GameObject CurrentWeaponSkill = GameObject.Find("Skill Image");
        GameObject SkillCircle = GameObject.Find("Skill Time Circle");
        GameObject SkillCoolTimeText = GameObject.Find("Skill Time Text");

        HP_ProgressBar = HPProgressBar.GetComponent<Slider>();
        MP_ProgressBar = MPProgressBar.GetComponent<Slider>();
        HaveMoney = Gold.GetComponentInChildren<TextMeshProUGUI>();
        HaveHPPotion = HPPotion.GetComponent<TextMeshProUGUI>();
        HaveMPPotion = MPPotion.GetComponent<TextMeshProUGUI>();
        Dodge_ProgressBar = DodgeProgressBar.GetComponent<Slider>();
        CurWeaponType = CurrentWeapon.GetComponent<Image>();
        Skill_Image = CurrentWeaponSkill.GetComponent<Image>();
        Skill_CoolDownCircle = SkillCircle.GetComponent<Image>();
        SkillCoolTime = SkillCoolTimeText.GetComponent<TextMeshProUGUI>();

        OnInitialized();
    }

    private void Update()
    {
        DodgeProgressCharge();
        AbilityUI_Updater();
    }

    #region Utility
    PlayerBase GetPlayerCharacter()
    {
        return Owner as PlayerBase;
    }

    PlayerSpecData GetPlayerSpec()
    {
        return GetPlayerCharacter().PlayerSpec;
    }
    #endregion

    #region Set UI
    public void SetMPRate()
    {
        if (!UtilityLibrary.IsValid(MP_ProgressBar))
        {
            Debug.Log(Owner.CommonSpec.GetCharacterName + " MP UI is Not Valid");
            return;
        }

        float MPRate = GetOwnerSpec().GetCurMP / GetOwnerSpec().GetMaxMP;
        MP_ProgressBar.value = MPRate;
    }

    void SetMoneyText()
    {
        if (!UtilityLibrary.IsValid(HaveMoney))
        {
            Debug.Log(Owner.CommonSpec.GetCharacterName + " Money UI is Not Valid");
            return;
        }

        int CurMoney = GetPlayerSpec().GetGold;
        string Text = CurMoney.ToString();
        HaveMoney.SetText(Text);
    }

    void SetHPPotionText()
    {
        if (!UtilityLibrary.IsValid(HaveHPPotion))
        {
            Debug.Log(Owner.CommonSpec.GetCharacterName + " HP Potion UI is Not Valid");
            return;
        }

        int CurPotion = GetPlayerSpec().GetHasHPPotion;
        HaveHPPotion.SetText(CurPotion.ToString());
    }

    void SetMPPotionText()
    {
        if (!UtilityLibrary.IsValid(HaveMPPotion))
        {
            Debug.Log(Owner.CommonSpec.GetCharacterName + " MP Potion UI is Not Valid");
            return;
        }

        int CurPotion = GetPlayerSpec().GetHasMPPotion;
        HaveMPPotion.SetText(CurPotion.ToString());
    }
    #endregion

    #region Dodge UI
    void InitDodge()
    {
        Dodge_ProgressBar.value = 1;
    }

    void DodgeStart()
    {
        DodgeTimer = 0;
        DodgeCaculateTrigger = true;
        Dodge_ProgressBar.value = 0;
    }

    void DodgeProgressCharge()
    {
        if(DodgeCaculateTrigger)
        {
            if(DodgeTimer < GetPlayerCharacter().AbilitySpec.GetDodgeCoolTime)
            {
                DodgeTimer += Time.deltaTime;
            }
            else
            {
                DodgeCaculateTrigger = false;
            }

            Dodge_ProgressBar.value = DodgeTimer/GetPlayerCharacter().AbilitySpec.GetDodgeCoolTime;
        }
    }
    #endregion

    #region Weapon Type UI
    void WeaponIconChangeToStart()
    {
        switch (GetPlayerCharacter().CurWeapon)
        {
            case EWeaponState.Sword:
                CurWeaponType.sprite = MeleeTypeIcon;
                Skill_Image.sprite = MeleeTypeSkill_Icon;
                break;

            case EWeaponState.Pistol:
                CurWeaponType.sprite = RangeTypeIcon;
                Skill_Image.sprite = RangeTypeSkill_Icon;
                break;
        }
    }

    void WeaponIconChange()
    {
        switch(GetPlayerCharacter().CurWeapon)
        {
            case EWeaponState.Sword:
                CurWeaponType.sprite = MeleeTypeIcon;
                Skill_Image.sprite = MeleeTypeSkill_Icon;
                break;

            case EWeaponState.Pistol:
                CurWeaponType.sprite = RangeTypeIcon;
                Skill_Image.sprite = RangeTypeSkill_Icon;
                break;
        }

        GetPlayerCharacter().PlaySoundSelect(WeaponChangeSound, EVolumeType.Effect);
    }
    #endregion

    #region Skill
    void AbilityUI_Updater()
    {
        // ���� ���¿� ���� ��ų�� ������ ����, ��Ÿ���� ������. ��� ��ü�� �÷��̾ ��
        switch (GetPlayerCharacter().CurWeapon)
        {
            case EWeaponState.Sword: // Ability 01
                AbilityStateChecker(GetPlayerCharacter().Ability01Flag, GetPlayerCharacter().Ability_01_Timer, GetPlayerCharacter().GetAbility01TimerProgress());
                break;

            case EWeaponState.Pistol: // Ability 02
                AbilityStateChecker(GetPlayerCharacter().Ability02Flag, GetPlayerCharacter().Ability_02_Timer, GetPlayerCharacter().GetAbility02TimerProgress());
                break;
        }
    }

    void AbilityStateChecker(bool AbilityFlag, float CurCoolTime, float CoolDownCircleProgress)
    {
        if (!AbilityFlag) // �÷��װ� ���� ���¿����� ����
        {
            if (!SkillCoolTime.enabled)
            {
                SkillCoolTime.enabled = true;
            }

            Skill_CoolDownCircle.fillAmount = CoolDownCircleProgress;
            string TimerText = Mathf.CeilToInt(CurCoolTime).ToString();
            SkillCoolTime.SetText(TimerText);
        }
        else // ���� �÷��װ� �����ִ� (= ��� ������ ���´�)
        {
            if(SkillCoolTime.enabled)
            {
                SkillCoolTime.enabled = false;
                Skill_CoolDownCircle.fillAmount = 0;
            }
        }
    }
    #endregion
}
