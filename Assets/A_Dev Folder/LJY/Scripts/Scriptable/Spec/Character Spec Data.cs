using UnityEngine;
using Project.Enums;
using Unity.VisualScripting;

public class CharacterSpecData : ScriptableObject
{
    public CharacterBase Owner;

    #region Spec Value
    [Header("Common Spec")]
    // =========================================================== // Name
    [SerializeField] string CharacterName;
    public string GetCharacterName { get { return CharacterName; } }
    // =========================================================== // HP
    [SerializeField] float MaxHP;
    protected float CurHP;
    public float GetMaxHP { get { return MaxHP; } }
    public float GetCurHP { get { return CurHP; } }
    // =========================================================== // MP
    [SerializeField] float MaxMP;
    protected float CurMP;
    public float GetMaxMP { get { return MaxMP; } }
    public float GetCurMP { get { return CurMP; } }
    // =========================================================== // ATK
    [SerializeField] float PureATK;
    float FinalATK;
    public float GetATK { get { return PureATK; } }
    public float GetFinalATK { get { return FinalATK; } }
    // =========================================================== // DEF
    [SerializeField] float PureDEF;
    float FinalDEF;
    public float GetDEF { get { return PureDEF; } }
    public float GetFinalDEF { get { return FinalDEF; } }
    // =========================================================== // Critical
    [SerializeField] float CriticalRate;
    [SerializeField] float CriticalDamage;
    public float GetCriticalRate { get { return CriticalRate; } }
    public float GetCriticalDamage { get { return CriticalDamage; } }
    // =========================================================== // Move Speed
    [SerializeField] float WalkSpeed;
    [SerializeField] float DashSpeed;
    float MasterMoveSpeed = 1;
    [SerializeField] float JumpForce;
    [SerializeField] float SecondJumpForceRate = 75f; // 기본 값
    [SerializeField] float WaterMoveDemerit = 75f;
    public float GetWalkSpeed { get { return WalkSpeed * GetMasterMoveSpeed(); } }
    public float GetDashSpeed { get { return DashSpeed * GetMasterMoveSpeed(); } }
    public float GetJumpForce()
    {
        if (Owner.CompareTag("Player"))
        {
            if (Owner.CurMoveMode == MovementMode.Ground)
            {
                return JumpForce;
            }
            else if (Owner.CurMoveMode == MovementMode.Water)
            {
                return JumpForce * (WaterMoveDemerit / 100); // 점프력 절반
            }
        }

        // 적들의 경우는 물에서 디메리트를 받지 않음
        return JumpForce;
    }
    public float GetSecondJumpForce { get { return JumpForce * (SecondJumpForceRate / 100); } }
    public virtual float GetMasterMoveSpeed()
    {
        if (Owner.CompareTag("Player"))
        {
            if (Owner.CurMoveMode == MovementMode.Ground)
            {
                return MasterMoveSpeed;
            }
            else if(Owner.CurMoveMode == MovementMode.Water)
            {
                return MasterMoveSpeed * (WaterMoveDemerit / 100); // 이동속도 절반
            }
        }

        // 적들의 경우는 물에서 디메리트를 받지 않음
        return MasterMoveSpeed;
    }
    // =========================================================== // Hit Recovery
    [SerializeField] float HitInvincibleTime;
    public float GetHitInvincibleTime { get { return HitInvincibleTime; } }
    // =========================================================== // Recovery Rates
    [SerializeField] protected float RecoveryRate = 1;
    [SerializeField] float MPGainRate = 1;

    public float GetRecoveryRate => RecoveryRate;
    public float GetMPGainRate => MPGainRate;
    #endregion

    #region Sound List
    [Header("Common Voice")]
    public AudioClip[] LightAttackAudioCue;
    public AudioClip[] HeavyAttackAudioCue;
    public AudioClip[] DamagedAudioCue;
    public AudioClip[] JumpAudioCue;
    #endregion

    public void SpecInitalize()
    {
        CurHP = GetMaxHP;
        CurMP = GetMaxMP;
        FinalATK = GetATK;
        FinalDEF = GetDEF; // 최초 최종 공/방은 퓨어값
    }

    #region Attribute Value Change
    public void DamageTaken(float DamageAmount)
    {
        if(Owner.CurStatus == EStatusEffect.Bleeding)
        {
            DamageAmount *= 1.3f;
        }

        CurHP = Mathf.Clamp(CurHP - DamageAmount, 0, MaxHP);

        if(CurHP <= 0)
        {
            Owner.CharacterDead();
            return;
        }

        StatusChanged();
    }

    public void DamageTakenToEffect(float DamageAmount)
    {
        // 효과로 인한 피해로 캐릭터가 죽지는 않음
        CurHP = Mathf.Clamp(CurHP - DamageAmount, 1, MaxHP);

        StatusChanged();
    }

    public void HealTaken(float HealAmount, EHealType PotionType)
    {
        float NewHealAmount = HealAmount * RecoveryRate;

        // 이후 아이템에 타입 정보등을 추가하면 열거형 정상 활용 (현재는 1번이 체력, 2번이 회복 고정)
        if (PotionType == EHealType.HPType)
        {
            CurHP = Mathf.Clamp(CurHP + NewHealAmount, 0, MaxHP);
        }

        else if (PotionType == EHealType.MPType)
        {
            CurMP = Mathf.Clamp(CurMP + NewHealAmount, 0, MaxMP);
        }

        StatusChanged();
    }

    public bool TakeCostMP(float CostAmount)
    {
        if (CurMP <= CostAmount)
        {
            return false;
        }

        CurMP = Mathf.Clamp(CurMP - CostAmount, 0, MaxMP);

        if(Owner.CompareTag("Player"))
        {
            // 적은 총알 발사에 코스트를 따로 사용하지 않음
            StatusChanged();
        }

        return true;
    }

    public void MPGain()
    {
        float Amount = 0;
        Amount = MaxMP / 100; // 최대 MP의 1%
        Amount *= MPGainRate * RecoveryRate;

        CurMP = Mathf.Clamp(CurMP + Amount, 0, MaxMP);
        StatusChanged();
    }
    #endregion

    #region Status Change Delegate Caller
    protected void StatusChanged()
    {
        if (!UtilityLibrary.IsValid(Owner))
        {
            Debug.Log("Common Spec Owner Is Not Valid");
            return;
        }

        if(UtilityLibrary.IsMob(Owner) && Owner.IsDead)
        {
            Debug.Log("Dead");
            return;
        }

        Owner.GetCharacterUIComponent().OnStatusChanged();
    }
    #endregion

    #region Additive Status
    [Header("[Debug] Additive Spec")]
    // =========================================================== //
    #region HP
    [SerializeField] float AdditiveHP = 0; // 말 그대로 추가된 수치
    public float GetAdditiveHP => AdditiveHP;
    public void SetAdditiveHP(float AdditiveAmount)
    {
        AdditiveHP += AdditiveAmount;

        Debug.Log("Debug - Before MaxHP: " + MaxHP);
        MaxHP += AdditiveAmount;
        CurHP += AdditiveAmount;
        Debug.Log("Debug - After MaxHP: " + MaxHP);
    }
    #endregion
    // =========================================================== //
    #region MP
    [SerializeField] float AdditiveMP = 0;
    public float GetAdditiveMP => AdditiveMP;
    public void SetAdditiveMP(float AdditiveAmount)
    {
        AdditiveMP += AdditiveAmount;

        Debug.Log("Debug - Before MaxMP: " + MaxMP);
        MaxMP += AdditiveAmount;
        MaxMP += AdditiveAmount;
        Debug.Log("Debug - After MaxMP: " + MaxMP);
    }
    #endregion
    // =========================================================== //
    #region ATK
    [SerializeField] float AdditiveATK = 0;
    public float GetAdditiveATK => AdditiveATK;
    public void SetAdditiveATK_ToAdd(float AdditiveAmount)
    {
        AdditiveATK += AdditiveAmount;

        Debug.Log("Debug - Before FinalATK: " + FinalATK);
        FinalATK += AdditiveAmount;
        Debug.Log("Debug - After FinalATK: " + FinalATK);
    }
    public void SetAdditiveATK_ToMultifly(float AdditiveAmountRate)
    {
        float Percent = PureATK / 100;
        AdditiveATK += Percent * AdditiveAmountRate;

        Debug.Log("Debug - Before FinalATK: " + FinalATK);
        FinalATK += Percent * AdditiveAmountRate;
        Debug.Log("Debug - After FinalATK: " + FinalATK);
    }
    #endregion
    // =========================================================== //
    #region DEF
    [SerializeField] float AdditiveDEF = 0;
    public float GetAdditiveDEF => AdditiveDEF;
    public void SetAdditiveDEF_ToAdd(float AdditiveAmount)
    {
        AdditiveDEF += AdditiveAmount;

        Debug.Log("Debug - Before FinalDEF: " + FinalDEF);
        FinalDEF += AdditiveAmount;
        Debug.Log("Debug - After FinalDEF: " + FinalDEF);
    }
    public void SetAdditiveDEF_ToMultifly(float AdditiveAmountRate)
    {
        float Percent = PureDEF / 100;
        AdditiveDEF += Percent * AdditiveAmountRate;

        Debug.Log("Debug - Before FinalDEF: " + FinalDEF);
        FinalDEF += Percent * AdditiveAmountRate;
        Debug.Log("Debug - After FinalDEF: " + FinalDEF);
    }
    #endregion
    // =========================================================== //
    #region Critical Rate
    [SerializeField] float AdditiveCriticalRate = 0;
    public float GetAdditiveCriticalRate => AdditiveCriticalRate;
    public void SetAdditiveCriticalRate(float AdditiveAmount)
    {
        AdditiveCriticalRate += AdditiveAmount;

        Debug.Log("Debug - Before Critical Rate: " + CriticalRate);
        CriticalRate += AdditiveAmount;
        Debug.Log("Debug - After Critical Rate: " + CriticalRate);
    }
    #endregion
    // =========================================================== //
    #region Critical Damage
    [SerializeField] float AdditiveCriticalDamage = 0;
    public float GetAdditiveCriticalDamage => AdditiveCriticalDamage;
    public void SetAdditiveCriticalDamage(float AdditiveAmount)
    {
        AdditiveCriticalDamage += AdditiveAmount;

        Debug.Log("Debug - Before Critical Damage: " + CriticalDamage);
        CriticalDamage += AdditiveAmount;
        Debug.Log("Debug - After Critical Damage: " + CriticalDamage);
    }
    #endregion
    // =========================================================== //
    #region MoveSpeed
    [SerializeField] float AdditiveMoveSpeed = 0;
    public float GetAdditiveMoveSpeed => AdditiveMoveSpeed;
    public void SetAdditiveMoveSpeed(float AdditiveAmount)
    {
        AdditiveMoveSpeed += AdditiveAmount;

        Debug.Log("Debug - Before Move Speed: " + MasterMoveSpeed);
        MasterMoveSpeed += AdditiveAmount;
        Debug.Log("Debug - After Move Speed: " + MasterMoveSpeed);
    }
    #endregion
    // =========================================================== //
    #endregion

}
