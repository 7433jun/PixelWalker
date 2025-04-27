using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Project.Enums;
using System.Collections;
using Project.Structs;
using static CharacterBase;
using UnityEngine.Timeline;
using Unity.Cinemachine;

public class CharacterBase : MonoBehaviour, UIComponentInterface
{
    #region Delegates

    #region OnDamaged
    public delegate void FOnDamaged(Vector2 ActionDir);
    public FOnDamaged OnDamaged;
    [HideInInspector] public bool IsDamaged;

    void DamagedAction(Vector2 ActionDir)
    {
        Debug.Log("OnDamaged!: " + CommonSpec.GetCharacterName);
        
        //GetCharacterUIComponent().SetHPRate();

        if(gameObject.CompareTag("Player"))
        {
            if(CameraShakeProfile && ImpulseSource)
            {
                CameraManager.Instance.CameraShakeFromProfile(CameraShakeProfile, ImpulseSource);
            }
            else if(!ImpulseSource)
            {
                Debug.Log("��� ��ü ����");
            }
        }

        Vector2 Dir = ActionDir.x > 0 ? Vector2.right : Vector2.left;
        if (Dir.x == FacingDir)
        {
            // �ٶ󺸴� ����� ���� ������ ������ ������ (���ݹ���� �ٶ󺸴� ������ ���� �����ؾ���)
            Flip();
        }

        if (CommonSpec.GetCurHP <= 0) // ���� ü���� 0 ���ϸ� ���
        {
            CharacterDead();
        }
        else // ���� ����
        {
            if (!ActorHasTag(Tag_SuperArmor) && ActionDir != Vector2.zero) // ���۾ƸӰ� ������ �ǰ� �׼��� ��Ÿ�� �ʿ�� ����
            {
                IsDamaged = true;

                ChangeStateToDamged();

                Dir += Vector2.up; // ���ݹ��� ������ �����ϸ� �� �밢���� Ƣ����

                rb.AddForce(Dir * DamagedKnockbackForce, ForceMode2D.Impulse);
            }

        }
    }
    #endregion

    #region OnDamaged By Object
    public delegate void FOnDamaged_ByObj();
    public FOnDamaged_ByObj OnDamaged_ByObj;

    void DamagedActionByObj()
    {
        Debug.Log("OnDamaged!: " + CommonSpec.GetCharacterName);

        //GetCharacterUIComponent().SetHPRate();

        if (gameObject.CompareTag("Player"))
        {
            CameraManager.Instance.CameraShakeFromProfile(CameraShakeProfile, ImpulseSource);
        }

        // ���� ������ ������ ���ư� �� �ֵ���
        Vector2 Dir = FacingDir > 0 ? Vector2.left : Vector2.right;

        if (CommonSpec.GetCurHP <= 0) // ���� ü���� 0 ���ϸ� ���
        {
            CharacterDead();
        }

        else // ���� ����
        {
            if (!ActorHasTag(Tag_SuperArmor)) // ���۾ƸӰ� ������ �ǰ� �׼��� ��Ÿ�� �ʿ�� ����
            {
                IsDamaged = true;

                ChangeStateToDamged();

                Dir += Vector2.up; // ���ݹ��� ������ �����ϸ� �� �밢���� Ƣ����

                rb.AddForce(Dir * DamagedKnockbackForce, ForceMode2D.Impulse);
            }

        }
    }
    #endregion

    #endregion

    #region Interface
    public virtual UIComponentBase GetCharacterUIComponent()
    {
        return null;
    }

    #endregion

    #region Scriptable Object
    public CharacterSpecData CommonSpec;
    public CharacterAbilitySpec AbilitySpec;
    #endregion

    #region Camera Shake
    [Header("Unique Camera Shake")]
    public CameraShakeProfile CameraShakeProfile;
    public CinemachineImpulseSource ImpulseSource { get; private set; }
    #endregion

    #region Components
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public BoxCollider2D CharacterCollider;
    public Animator Anim { get; private set; }
    public AudioSource AudioComp { get; private set; }
    public GenericGroundDetector GroundDetectorComp { get; private set; }
    public MeleeHitBoxController MeleeHitBox_Controller { get; private set; }
    public AfterImageEffect AfterImageEffector { get; private set; }

    public MasterStateMachine UsingStateMachine { get; private set; }
    #endregion

    #region Generic Utility

    #region Status Effect
    // ���� �̻� ���� ��Ģ
    /*
        1. ���ÿ� ����� �� �ִ� ���� �̻��� �� 1��
        2. ������ ����� ���� �̻��� �����Ѵٸ� ���� Ÿ���� �ƴ� �̻� ������ �߰� �������� ������ ���ο� ���� �̻��� ���õ�
        3. ��ĳ���� ����: �ִ� ������ 4����
        4. ���� �Ǿ��ִ� ���� �̻��� �� ����� ��� �ִ� ���ñ��� ������ ������, ���� ���̴� ���� �̻� ȿ���� �ߴܵǰ� ����� (���� �ð� �ʱ�ȭ)
        5. ������ �ٲ�� �Դ� ���ط��� �޶������� �ΰ� ȿ�� �� ���� �ð� � ������ ����

        * ���� �ð��� ���� ������ ȿ���� �Ŵ� �����ڸ��� �ٸ��� ���� ���� �Ѱ��� �� ���� *

        ���� ȿ��) (�Һ�) �����κ��� �޴� ���� 30% ���� + (1���� ����) �ִ� ü���� 1% ���� (�⺻ ���� ���� | ���� �ð� 10�� / ���� ���� 2��)
        �ߵ� ȿ��) (1���� ����) �ִ� ü�� 3% ���� (�⺻ ���� ���� | ���� �ð� 10�� / ���� ���� 2��)

        ���� �߻� ��) ���� ���ÿ� ���� ���� �ð����� �����Ѵٰ� �ϸ� �Ʒ� ApplyStatus ���ο��� ���� ���ڸ� �ް� �ð��� �÷��� ��.
        ������ �� �ִ� ���� �̻��� ���� �÷����ϸ�? ����Ʈ�� �迭�� ���� ���� �޴� ��� ��
    */

    public EStatusEffect CurStatus = EStatusEffect.None; // { get; private set; }
    Coroutine CurStatusEffectEventHandle = null;
    int MaxStack = 4;
    int CurStack = 0;

    public void ChangeStatus(EStatusEffect NewStatus)
    {
        CurStatus = NewStatus;

        if(NewStatus == EStatusEffect.None)
        {
            // ���� �ʱ�ȭ �ÿ��� 0����
            CurStack = 0;
        }
    }

    public bool IsAppliedEffect()
    {
        return CurStatus!= EStatusEffect.None;
    }

    void StopStatusEffect()
    {
        if(CurStatusEffectEventHandle != null)
        {
            StopCoroutine(CurStatusEffectEventHandle);
        }
    }

    void CaculatingStatusEffectStack(EStatusEffect ApplyStatusEffectType)
    {
        if (CurStatus == EStatusEffect.None)
        {
            // ���� ���� ����
            if(CurStack != 1)
            {
                CurStack = 1;
            }
        }
        else if (CurStatus == ApplyStatusEffectType)
        {
            // ���� ����
            CurStack = Mathf.Clamp(CurStack + 1, 2, MaxStack);
            Debug.Log(CurStack + " | " + MaxStack);
            StopStatusEffect();
        }
        else if(CurStatus != ApplyStatusEffectType)
        {
            
        }
    }

    public void ApplyStatusEffectToSelf(EStatusEffect EffectType, float DurationSpec, float PeriodSpec)
    {
        switch (EffectType)
        {
            case EStatusEffect.Bleeding:

                CaculatingStatusEffectStack(EStatusEffect.Bleeding);

                Debug.Log("���� ���� ����: " + CurStack);

                CurStatusEffectEventHandle = StatusEffectManager.Instance.BleedingMasterEffector.DOT_EffectSpecDefinitionAndApply(this,
                    EffectType, DurationSpec, PeriodSpec, CurStack);

                break;

            case EStatusEffect.Posioning:

                CaculatingStatusEffectStack(EStatusEffect.Posioning);

                CurStatusEffectEventHandle = StatusEffectManager.Instance.BleedingMasterEffector.DOT_EffectSpecDefinitionAndApply(this,
                    EffectType, DurationSpec, PeriodSpec, CurStack);

                break;

            default:
                Debug.Log("Invalid Effect Set");
                break;
        }
    }

    public void ApplyStatusEffectToTarget(CharacterBase Target, EStatusEffect EffectType, float DurationSpec, float PeriodSpec)
    {
        switch (EffectType)
        {
            case EStatusEffect.Bleeding:

                CaculatingStatusEffectStack(EStatusEffect.Bleeding);

                CurStatusEffectEventHandle = StatusEffectManager.Instance.BleedingMasterEffector.DOT_EffectSpecDefinitionAndApply(Target, EffectType,
                    DurationSpec, PeriodSpec, CurStack);

                break;

            case EStatusEffect.Posioning:

                CaculatingStatusEffectStack(EStatusEffect.Posioning);

                CurStatusEffectEventHandle = StatusEffectManager.Instance.BleedingMasterEffector.DOT_EffectSpecDefinitionAndApply(Target, EffectType,
                    DurationSpec, PeriodSpec, CurStack);

                break;

            default:
                Debug.Log("Invalid Effect Set");
                break;
        }
    }
    #endregion

    #region Movement Mode
    public MovementMode CurMoveMode = MovementMode.Ground; //{ get; private set; } = MovementMode.Ground;

    public void ChangeMoveMode(MovementMode ChangeMode)
    {
        CurMoveMode = ChangeMode;
    }

    #endregion

    #region Actor Tag
    public List<string> ActorTag = new List<string>();

    #region Tag List
    public string Tag_SuperArmor { get; private set; } = "Super Armor";
    public string Tag_Invincible { get; private set; } = "Invincible";
    public string Tag_PerfectDodge { get; private set; } = "PerfectDodge";
    public string Tag_DetectorDisable { get; private set; } = "Detector Disable";
    public string Tag_Boss { get; private set; } = "Boss";
    #endregion

    #region Actor Tag Functions

    public bool ActorHasTag(string TagToCheck)
    {
        return ActorTag.Contains(TagToCheck);
    }

    public void AddTag(string AddTag)
    {
        if (ActorHasTag(AddTag))
        {
            //Debug.Log("Already Has Tag");

            return;
        }

        ActorTag.Add(AddTag);
    }

    public void RemoveTag(string RemoveTag)
    {
        if (!ActorHasTag(RemoveTag))
        {
            //Debug.Log("Not Has Tag");

            return;
        }

        ActorTag.Remove(RemoveTag);
    }
    #endregion

    #endregion

    #region Facing Direction Info
    public float FacingDir { get; private set; } = 1;
    public bool FacingRight { get; private set; } = true;
    #endregion

    #region Flip
    public void Flip()
    {
        FacingDir = FacingDir * -1;
        FacingRight = !FacingRight;
        transform.Rotate(0, 180, 0);
        GroundDetectorComp.FlipDetectorOffset();
    }

    void ReverseArrangement()
    {
        FacingDir = FacingDir * -1;
        FacingRight = !FacingRight;
        GroundDetectorComp.FlipDetectorOffset();
    }

    public void FlipController(float X)
    {
        if (X > 0 && !FacingRight)
        {
            Flip();
        }

        else if (X < 0 && FacingRight)
        {
            Flip();
        }
    }
    #endregion

    #region Projectile Spawn Point
    Transform SpawnBulletPoint;
    #endregion

    #region Velocity
    public void SetVelocity_XAxis_WithFlip(Vector2 MoveVector)
    {
        rb.linearVelocityX = MoveVector.x;
        FlipController(MoveVector.x);
    }

    public void SetVelocity_Just_XAxis(float XVelo)
    {
        rb.linearVelocityX = XVelo;
    }

    public void SetVelocity(float X, float Y)
    {
        rb.linearVelocityX = X;
        rb.linearVelocityY = Y;

        FlipController(X);
    }

    public void JumpVelocity()
    {
        if(!bIsDoubleJump)
        {
            rb.linearVelocityY = CommonSpec.GetJumpForce();
        }
        else
        {
            rb.linearVelocityY = CommonSpec.GetSecondJumpForce;
        }
    }

    public void ZeroVelocity() => rb.linearVelocity = new Vector2(0, 0);
    public void ZeroVelocityToX() => rb.linearVelocityX = 0;

    #endregion

    #endregion

    #region Sub Utility

    #region Support Coroutine
    IEnumerator HitInvincibleCancel(float InvincibleDuration)
    {
        yield return new WaitForSeconds(InvincibleDuration);

        IsDamaged = false;
        DisableInvincible();
    }
    #endregion

    #region State Setter

    void ChangeStateToDamged()
    {
        if (HasDamagedAnims > 1)
        {
            // �⺻ ���� 1���� �� ���� ��츦 ���� ���
            float RandomFloat = UtilityLibrary.RandomBool() ? 1f : -1f;
            Anim.SetFloat("Damaged List", RandomFloat);
        }

        UsingStateMachine.ChangeState(_DamagedState);
    }

    #region Invincible Set
    public void EnableInvincible()
    {
        AddTag(Tag_Invincible);
        sr.color = new Color(1, 1, 1, 0.6f);
    }

    public void DisableInvincible()
    {
        RemoveTag(Tag_Invincible);
        sr.color = new Color(1, 1, 1, 1);
    }
    public void ClearInvincible(float InvincibleDuration)
    {
        StartCoroutine(HitInvincibleCancel(InvincibleDuration));
    }

    #endregion

    #endregion

    #endregion

    #region Category: Animation

    #region Animation Utility
    [Header("Has Sub Anim Num")]
    public int HasIdleAnims = 1;
    public int HasPrimaryAttackCountToSword;
    public int HasDamagedAnims = 1;

    bool RememberSwordCombo = false;

    float AttackRememberTime = .4f;
    float ElapsedTime = 0;

    #region Attack Remember
    void SwordComboRemember()
    {
        if (ElapsedTime < AttackRememberTime)
            ElapsedTime += Time.deltaTime;

        else
        {
            ElapsedTime = 0;
            SwordAttackCount = 0;
            RememberSwordCombo = false;
        }
    }
    #endregion

    public void ReturnBasicAnim(string AnimBoolName)
    {
        Anim.SetBool(AnimBoolName, false);
    }
    #endregion

    #region Anim Notify

    #region Play Sound
    public void PlaySoundSelect(AudioClip SelectSound, EVolumeType VolumeType)
    {
        UtilityLibrary.PlaySoundAtCharacter(SelectSound, VolumeType, this);
    }

    public void Notify_PlaySoundSelectToEffect(AudioClip SelectSound)
    {
        UtilityLibrary.PlaySoundAtCharacter(SelectSound, EVolumeType.Effect, this);
    }

    public void PlaySoundSelectToVoice(AudioClip SelectSound)
    {
        UtilityLibrary.PlaySoundAtCharacter(SelectSound, EVolumeType.Voice, this);
    }
    public void PlaySoundLightAttack()
    {
        int Idx = 0;
        if (CommonSpec.LightAttackAudioCue.Length > 1)
        {
            Idx = Random.Range(0, CommonSpec.LightAttackAudioCue.Length);
        }

        if (!UtilityLibrary.IsValid(CommonSpec.LightAttackAudioCue[Idx]))
        {
            Debug.Log("Not Setting Light Attack Voice! To " + CommonSpec.GetCharacterName);
        }

        UtilityLibrary.PlaySoundAtCharacter(CommonSpec.LightAttackAudioCue[Idx], EVolumeType.Voice, this);
    }
    public void PlaySoundHeavyAttack()
    {
        int Idx = 0;
        if (CommonSpec.HeavyAttackAudioCue.Length > 1)
        {
            Idx = Random.Range(0, CommonSpec.HeavyAttackAudioCue.Length);
        }

        if (!UtilityLibrary.IsValid(CommonSpec.HeavyAttackAudioCue[Idx]))
        {
            Debug.Log("Not Setting Heavy Attack Voice! To " + CommonSpec.GetCharacterName);
        }

        UtilityLibrary.PlaySoundAtCharacter(CommonSpec.HeavyAttackAudioCue[Idx], EVolumeType.Voice, this);
    }
    public void PlaySoundJump()
    {
        int Idx = 0;
        if (CommonSpec.JumpAudioCue.Length < 1)
        {
            return;
        }

        if (CommonSpec.JumpAudioCue.Length > 1)
        {
            Idx = Random.Range(0, CommonSpec.JumpAudioCue.Length);
        }

        if (!UtilityLibrary.IsValid(CommonSpec.JumpAudioCue[Idx]))
        {
            Debug.Log("Not Setting Light Attack Voice! To " + CommonSpec.GetCharacterName);
            return;
        }

        UtilityLibrary.PlaySoundAtCharacter(CommonSpec.JumpAudioCue[Idx], EVolumeType.Voice, this);
    }
    public void PlaySoundOnDamaged()
    {
        int Idx = 0;
        if (CommonSpec.DamagedAudioCue.Length > 1)
        {
            Idx = Random.Range(0, CommonSpec.DamagedAudioCue.Length);
        }

        if (!UtilityLibrary.IsValid(CommonSpec.DamagedAudioCue[Idx]))
        {
            Debug.Log("Not Setting Light Attack Voice! To " + CommonSpec.GetCharacterName);
        }

        UtilityLibrary.PlaySoundAtCharacter(CommonSpec.DamagedAudioCue[Idx], EVolumeType.Voice, this);
    }
    #endregion

    #region Sword Primary Attack Notify
    public void ClearEventHandle_SwordComboReset()
    {
        // ùŸ�� ��Ÿ�� ������ �޺� ��� ���Կ� ������ ��
        RememberSwordCombo = false;
        ElapsedTime = 0;
    }

    public void PrimaryAttackActionEndToSword()
    {
        UsingStateMachine.ChangeState(_IdleState);

        if (SwordAttackCount == HasPrimaryAttackCountToSword || SwordAttackCount == 0) // ó�� �������� ���� ī��Ʈ�� ����/�����ϰ� ���� ������ 0ó��. �⺻ ������ ������ġ
        {
            // ��Ÿ�� ���� �ʱ�ȭ
            SwordAttackCount = 0;
            RememberSwordCombo = false;
            ElapsedTime = 0;
            return;
        }

        if (RememberSwordCombo)
            ElapsedTime = 0;

        RememberSwordCombo = true;
    }

    public void SetSwordHitBoxSize()
    {
        MeleeHitBox_Controller.DamageRateIdx = 0; // �÷��̾�� �������, ���� ��� ������ 0��° �ε����� ��Ÿ ���ط�
        MeleeHitBox_Controller.SetMeleeBoxActivate(true);
        // �̹� ������ �����ϰ� ��Ƽ���̷� �ٲٴ°Ŷ� ī��Ʈ -1
        MeleeHitBox_Controller.SetMeleeBoxSize(SwordHitBoxOffsetAndSize[Anim.GetInteger("Sword Combo Count")]);
    }

    public void SetSwordBoxDeactivate()
    {
        MeleeHitBox_Controller.SetMeleeBoxActivate(false);
    }
    #endregion

    #region Pistol Primary Attack Notify

    public void SpawnBullet(EProjectileType SearchType)
    {
        if (!CommonSpec.TakeCostMP(AbilitySpec.GetBulletCost))
        {
            return;
        }

        GameObject BulletToSpawn = BulletPool.BP_Instance.GetBulletTypeByBulletType(SearchType);

        if (UtilityLibrary.IsValid(BulletToSpawn))
        {
            Bullet SpawnedBulletData = BulletToSpawn.GetComponent<Bullet>();
            SpawnedBulletData.Owner = this;
            SpawnedBulletData.FireDir = FacingDir;
            SpawnedBulletData.LifeTime = SpawnedBulletData.LifePeriod;

            BulletToSpawn.transform.position = SpawnBulletPoint.position;

            if (FacingRight)
            {
                SpawnedBulletData.transform.eulerAngles = new Vector3(0, 0, 0);
                BulletToSpawn.SetActive(true);
            }

            else
            {
                SpawnedBulletData.transform.eulerAngles = new Vector3(0, 180, 0);
                BulletToSpawn.SetActive(true);
            }
        }
    }
    #endregion

    #region OnDead Notify
    public void DeadMobDestroy()
    {
        Destroy(gameObject);
    }

    public void MonsterItemDrop()
    {
        // ToDo
        // ������ Ȯ���� ���� ��� ���̺� ���� ������ ���
    }
    #endregion

    #region Dodge Notify
    public void Notify_PerfectDodge()
    {
        // To Do
        // ����Ʈ ���
        Search_Effect(EEffectType.PerfectEvasion);
        // ���� ���
        
        // Ư�� ���� �ߵ�
    }

    public void Notify_JustDodge()
    {
        // To Do
        // ����Ʈ ���
        Search_Effect(EEffectType.JustEvasion);
        // ���� ���
    }

    public void Search_Effect(EEffectType SearchType)
    {
        GameObject EffectToSpawn = EffectPool.Instance.GetEffectTypeByEffectType(SearchType);

        if (UtilityLibrary.IsValid(EffectToSpawn))
        {
            EffectToSpawn.transform.position = transform.position;

            switch(SearchType)
            {
                case EEffectType.HP_Recovery:
                case EEffectType.MP_Recovery:
                    EffectToSpawn.transform.position -= new Vector3(0, 0.5f, 0);
                    // ü���̳� ���� ȸ���� ��� y�� ��¦ ���߱�
                    break;
            }

            EffectToSpawn.SetActive(true);
        }
    }
    #endregion

    public void GenericActionEnd()
    {
        if(IsDamaged)
        {
            IsDamaged = false;
        }

        UsingStateMachine.ChangeState(_IdleState);
    }

    #endregion

    #endregion

    #region Category: Detector System

    [Header("Character Base 2D / Detector")]
    public bool bUseDrawDebug { get; private set; }

    public bool GroundDetect() => GroundDetectorComp.GroundDetecting();
    public bool SlopeDetect() => GroundDetectorComp.SlopeDetecting();
    public bool WallDetect() => GroundDetectorComp.WallDetecting();

    #endregion

    #region Category: Inspector Value

    #region Team ID
    [Header("Team ID")]
    public TeamID CharacterTeamID = TeamID.Neutrality; // �⺻�� �߸�
    #endregion

    #region Knockback
    [Header("Knockback Force")]
    [SerializeField] float DamagedKnockbackForce = 2.5f;
    #endregion

    #endregion

    #region Category: State

    #region State Variations
    public IdleState _IdleState { get; private set; }
    public MoveState _MoveState { get; private set; }
    public JumpState _JumpState { get; private set; }
    public AirborneState _AirborneState { get; private set; }
    public DodgeState _DodgeState { get; private set; }
    public MeleeAttackState _MeleeAttackState { get; private set; }
    public RangeAttackState _RangeAttackState { get; private set; }
    public DamagedState _DamagedState { get; private set; }
    public DeadState _DeadState { get; private set; }
    public GroundAbility_01 _GroundAbility_01 { get; private set; }
    public GroundAbility_02 _GroundAbility_02 { get; private set; }
    public GroundAbility_03 _GroundAbility_03 { get; private set; }

    void InitState()
    {
        _IdleState = new IdleState(this, UsingStateMachine, "Idle");
        _MoveState = new MoveState(this, UsingStateMachine, "Move");
        _JumpState = new JumpState(this, UsingStateMachine, "Jump");
        _AirborneState = new AirborneState(this, UsingStateMachine, "Jump"); // ����� ������ ��Ʈ
        _DodgeState = new DodgeState(this, UsingStateMachine, "Dodge");
        _MeleeAttackState = new MeleeAttackState(this, UsingStateMachine, "Sword Primary Attack");
        _RangeAttackState = new RangeAttackState(this, UsingStateMachine, "Pistol Primary Attack");
        _DamagedState = new DamagedState(this, UsingStateMachine, "Damaged");
        _DeadState = new DeadState(this, UsingStateMachine, "Dead");
        _GroundAbility_01 = new GroundAbility_01(this, UsingStateMachine, "Ability_01");
        _GroundAbility_02 = new GroundAbility_02(this, UsingStateMachine, "Ability_02");
        _GroundAbility_03 = new GroundAbility_03(this, UsingStateMachine, "Ability_03");
    }
    #endregion

    #region State Value

    #region State Category | Busy - Dead - Running
    [HideInInspector] public bool IsBusy = false;
    public bool IsDead { get; private set; } = false;
    [HideInInspector] public bool IsRunning = false;

    public void CharacterDead()
    {
        IsDead = true;
        UsingStateMachine.ChangeState(_DeadState);
        ZeroVelocity();
    }
    #endregion

    #region Equipped Weapon
    [HideInInspector] public EWeaponState CurWeapon = EWeaponState.Sword;
    #endregion

    #region Melee Attack
    [HideInInspector] public int SwordAttackCount = 0;
    #endregion

    #region Move Vector
    [HideInInspector] public Vector2 MoveVec;
    #endregion

    #region Dodge State
    public bool bCanDodge { get; private set; } = true;
    public void SetDodgeBoolean(bool StateCondition) => bCanDodge = StateCondition;
    #endregion

    #region Double Jump
    public bool bIsDoubleJump { get; private set; } = false;
    public void SetDoubleJumpCondition(bool Condition)
    {
        bIsDoubleJump = Condition;
    }
    #endregion

    #endregion

    #endregion

    #region Category: Platform
    public GameObject CurPlatform { get; private set; }
    public void GenericPlatformIgnore()
    {
        CompositeCollider2D PlatformCollider = CurPlatform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(CharacterCollider, PlatformCollider);
    }
    #endregion

    #region Category: Slope
    /**/
    #region Slope Boolean
    public bool OnSlope { get; private set; }
    public bool JumptToSlope { get; private set; }
    #endregion

    #region Jump To Slope Check
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            CurPlatform = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (UsingStateMachine.CurState == _JumpState)
        {
            // �浹�� ǥ���� ��� ���� Ȯ��
            Vector2 surfaceNormal = collision.contacts[0].normal;

            // ��Ż������ üũ (45�� = Vector2.up�� ��� ������ ������ 45��)
            JumptToSlope = Mathf.Abs(Vector2.Angle(Vector2.up, surfaceNormal) - 45f) < 0.1f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // ��Ż�� Ȥ�� �浹�� ����� ���� �ʱ�ȭ
        JumptToSlope = false;

        if (collision.gameObject.CompareTag("Platform"))
        {
            CurPlatform = null;
        }
    }
    #endregion

    #endregion

    #region Category: Hit Box
    [Header("Melee Area Setter")]
    public FSwordHitBox[] SwordHitBoxOffsetAndSize;
    public Vector2[] SwordBoxOffsets;
    public Vector2[] SwordBoxSizes;
    #endregion

    #region Engine Functions
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        CharacterCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        AudioComp = GetComponent<AudioSource>();
        GroundDetectorComp = GetComponentInChildren<GenericGroundDetector>();
        MeleeHitBox_Controller = GetComponentInChildren<MeleeHitBoxController>();
        AfterImageEffector = GetComponentInChildren<AfterImageEffect>();

        ImpulseSource = GetComponent<CinemachineImpulseSource>();

        CommonSpec.Owner = this;

        OnDamaged += DamagedAction;
        OnDamaged_ByObj = DamagedActionByObj;

        UsingStateMachine = new MasterStateMachine();
        InitState();

        CommonSpec.SpecInitalize();

        // ���� �˰� �����ŭ ũ�� ����
        SwordHitBoxOffsetAndSize = new FSwordHitBox[HasPrimaryAttackCountToSword];

        for (int i = 0; i < SwordHitBoxOffsetAndSize.Length; i++)
        {
            SwordHitBoxOffsetAndSize[i] = new FSwordHitBox
            {
                Offset = SwordBoxOffsets[i],
                Size = SwordBoxSizes[i]
            };
        }
    }

    protected virtual void Start()
    {
        UsingStateMachine.StateInitialize(_IdleState);

        SpawnBulletPoint = transform.Find("Bullet Spawn Point");

        if(transform.localEulerAngles.y > 179 && transform.localEulerAngles.y < 181)
        {
            Debug.Log("���� ��ġ");
            Quaternion ActorRotation = new Quaternion (0, 180, 0, 1);
            ReverseArrangement();
        }
    }

    protected virtual void FixedUpdate()
    {
        /*
        if (OnSlope && UsingStateMachine.CurState == _IdleState || UsingStateMachine.CurState == _SwordAttackState || UsingStateMachine.CurState == _PistolAttackState)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            // X�� ���� ���� (���� ���� ����)
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }

        if(OnSlope && UsingStateMachine.CurState == _MoveState)
        {
            rb.gravityScale = 5;
            //Spec.SetMoveSpeed(Spec.GetMasterMoveSpeed * rb.gravityScale);
        }

        else
        {
            rb.gravityScale = 1;
            //Spec.SetMoveSpeed(1);
        }
        */
    }

    protected virtual void Update()
    {
        UsingStateMachine.CurState.Update();

        if(RememberSwordCombo)
        {
            SwordComboRemember();
        }
        // Slope

        if (CompareTag("Player"))
        {
            if (UsingStateMachine.CurState == _IdleState || UsingStateMachine.CurState == _MoveState || UsingStateMachine.CurState == _MeleeAttackState || UsingStateMachine.CurState == _RangeAttackState)
            {
                OnSlope = SlopeDetect();
            }
            else
            {
                OnSlope = false;
                if (GroundDetect() && rb.gravityScale != 1 && !OnSlope)
                {
                    // ���� ���� ���� �߷� ���� 1�� �ƴ� ���
                    rb.gravityScale = 1;
                }
            }

            if (RememberSwordCombo)
            {
                SwordComboRemember();
            }
            else
            {
                ElapsedTime = 0;
            }
        }
    }
    #endregion

}
