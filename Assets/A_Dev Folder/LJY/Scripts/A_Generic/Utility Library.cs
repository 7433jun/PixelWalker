using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Project.Enums;
using UnityEngine.Rendering;
using Project.Structs;
using Unity.Cinemachine;

public static class UtilityLibrary
{
    #region Valid Check
    public static bool IsValid(Object TargetObject)
    {
        return TargetObject != null;
    }
    #endregion

    #region Camera Shake
    public static void SetCameraShakeOffset(ref FCameraShakeOffset CameraShake, float ImpulseDuration, float ShakeDuration, float MaxImpactForce, float Magnitude)
    {
        CameraShake = new FCameraShakeOffset();

        CameraShake.ImpulseDuration = ImpulseDuration;
        CameraShake.ShakeDuration = ShakeDuration;
        CameraShake.MaxImpactForce = MaxImpactForce;
        CameraShake.Magnitude = Magnitude;
    }

    public static void PlayCameraShake(CameraShakeProfile Profile, CinemachineImpulseSource ImpulseSource, FCameraShakeOffset OffsetData)
    {
        CameraManager.Instance.StartCameraShake(Profile, ImpulseSource,
                        OffsetData.ImpulseDuration,
                        OffsetData.ShakeDuration,
                        OffsetData.MaxImpactForce,
                        OffsetData.Magnitude);
    }
    #endregion

    #region Generic Utility
    public static void IgnorePlatform(GameObject Instigator)
    {
        
    }

    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 1;
    }

    public static int RandomIntegerValue(int Min, int Max)
    {
        if (Min > Max)
        {
            Debug.LogWarning("숫자값 지정 잘못됨");
            return 0;
        }

        if (Min == Max)
        {
            return Max;
        }

        int Value = Random.Range(Min, Max + 1);

        return Value;
    }
    #endregion

    #region Character Utility

    #region Search Player
    public static PlayerBase GetPlayerCharacterInGame()
    {
        GameObject SearchedTargetPlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerBase FoundPlayer = SearchedTargetPlayer.GetComponent<PlayerBase>();

        if(!IsValid(FoundPlayer))
        {
            return null;
        }

        return FoundPlayer;
    }

    public static PlayerController GetPlayerControllerInGame()
    {
        GameObject SearchedTargetPlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerController FoundController = SearchedTargetPlayer.GetComponent<PlayerController>();

        if (!IsValid(FoundController))
        {
            return null;
        }

        return FoundController;
    }

    #endregion

    #region Team Check
    static public bool IsHostile(CharacterBase Instigator, CharacterBase Target)
    {
        if (!IsValid(Instigator) || !IsValid(Target))
        {
            Debug.LogWarning("Instigator is invalid or Target is null");
            return false;
        }

        if(Instigator.CharacterTeamID != Target.CharacterTeamID && Target.CharacterTeamID != TeamID.Neutrality)
        {
            // 이벤트 발생자와 타겟의 팀 아이디가 다르면서 타겟 아이디가 중립이 아닐 때
            return true;
        }

        return false;
    }
    #endregion

    #region Character Position
    public static Vector2 GetActorLocationIn2D(CharacterBase InCharacter)
    {
        if (!IsValid(InCharacter))
        {
            Debug.LogError("InCharacter is invalid! Returning Vector2.zero.");
            return Vector2.zero;
        }

        return InCharacter.transform.position;
    }
    public static Vector2 GetActorForwardVectorIn2D(CharacterBase InCharacter)
    {
        if (!IsValid(InCharacter))
        {
            Debug.LogError("InCharacter is invalid! Returning Vector2.zero.");
            return Vector2.zero;
        }

        return (InCharacter.transform.right * InCharacter.FacingDir);
    }

    #endregion

    #region Character Type
    public static bool IsMob(CharacterBase SearchTarget)
    {
        if(SearchTarget.CompareTag("Enemy") && !SearchTarget.ActorHasTag("Boss"))
        {
            return true;
        }

        return false;
    }

    public static bool IsBoss(CharacterBase SearchTarget)
    {
        if (SearchTarget.CompareTag("Enemy") && SearchTarget.ActorHasTag("Boss"))
        {
            return true;
        }

        return false;
    }

    public static bool IsPlayer(CharacterBase SearchTarget)
    {
        if (SearchTarget.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Angle
    public static bool GetAngleIn2D(CharacterBase InCharacter, Vector2 TargetActorPosition, out float AngleValue)
    {
        AngleValue = 0;

        if (!IsValid(InCharacter))
        {
            Debug.LogError("InCharacter is invalid! Returning false.");
            return false;
        }

        Vector2 InstigatorForward = GetActorForwardVectorIn2D(InCharacter);
        Vector2 DirectionToTarget = (TargetActorPosition - GetActorLocationIn2D(InCharacter)).normalized;

        AngleValue = Vector2.Angle(InstigatorForward, DirectionToTarget);

        bool bIsBack = Vector2.Dot(InstigatorForward, DirectionToTarget) < 0;

        return bIsBack;
    }
    public static bool GetAngleIn2D(CharacterBase InCharacterA, CharacterBase InCharacterB, out float AngleValue)
    {
        AngleValue = 0;

        if (!IsValid(InCharacterA) || !IsValid(InCharacterB))
        {
            Debug.LogError("InCharacter is invalid! Returning false.");
            return false;
        }

        Vector2 InstigatorForward = GetActorForwardVectorIn2D(InCharacterA);
        Vector2 TargetPosition = GetActorLocationIn2D(InCharacterB);
        Vector2 DirectionToTarget = (TargetPosition - GetActorLocationIn2D(InCharacterA)).normalized;

        AngleValue = Vector2.Angle(InstigatorForward, DirectionToTarget);

        bool bIsBack = Vector2.Dot(InstigatorForward, DirectionToTarget) < 0;

        return bIsBack;
    }
    #endregion

    #region Find Nearest Target
    public static CharacterBase GetNearestTarget(CharacterBase Instigator, List<CharacterBase> Targets)
    {
        if (!IsValid(Instigator) || Targets == null || Targets.Count == 0)
        {
            Debug.LogError("Instigator is invalid or Targets is null or empty!");
            return null;
        }

        CharacterBase NearestTarget = null;
        float ShortestDistance = float.MaxValue;

        Vector2 CurrentPosition = Instigator.transform.position;

        foreach (var Target in Targets)
        {
            if (!IsValid(Target)) continue;

            float Distance = GetDistance_A_to_B(CurrentPosition, GetActorLocationIn2D(Target));

            if (Distance < ShortestDistance)
            {
                ShortestDistance = Distance;
                NearestTarget = Target;
            }
        }

        return NearestTarget;
    }
    #endregion

    #region Direction
    public static Vector2 GetDirection_A_to_B(GameObject Instigator, GameObject Target)
    {
        if (!IsValid(Instigator) || !IsValid(Target))
        {
            Debug.LogError("One or both characters are invalid! Returning Vector2.zero.");
            return Vector2.zero;
        }

        return (Target.transform.position - Instigator.transform.position).normalized;
    }
    #endregion

    #region Distance
    public static float GetDistance_A_to_B(CharacterBase InCharacterA, CharacterBase InCharacterB)
    {
        if (!IsValid(InCharacterA) || !IsValid(InCharacterB))
        {
            Debug.LogError("One or both characters are invalid! Returning 0.");
            return 0f;
        }
        return Vector2.Distance(InCharacterA.transform.position, InCharacterB.transform.position);
    }
    public static float GetDistance_A_to_B(Vector2 PointA, Vector2 PointB)
    {
        return Vector2.Distance(PointA, PointB);
    }
    #endregion

    #endregion

    #region Damage & Heal Caculate

    #region Utility
    static public void OnAttacked_InstigatorToTarget(CharacterBase Instigator, CharacterBase DamagedTarget, GameObject DirStartGameObject, float DamageRate)
    {
        if (DamagedTarget.ActorHasTag(DamagedTarget.Tag_PerfectDodge))
        {
            Debug.Log("Perfect Dodge!!");
            DamagedTarget.Notify_PerfectDodge();
            // 짧은 슬로우 효과
            // 버프
            return;
        }

        if (Instigator == DamagedTarget)
        {
            Debug.Log("Hit Self");
            return;
        }

        if (!DamagedTarget.ActorHasTag(DamagedTarget.Tag_Invincible) && IsHostile(Instigator, DamagedTarget)) // 무적이 아니며, 적대적일 때만
        {
            //float DamageRate = Instigator.AbilitySpec.GetBulletDamageRate;
            float DamageAmount = Instigator.CommonSpec.GetFinalATK * (DamageRate / 100);
            float GiveDamage = GenericCaculateDamage(Instigator, DamageAmount);

            TakeDamageToTarget(Instigator, DamagedTarget, GiveDamage);

            Vector2 ActionDir = GetDirection_A_to_B(DirStartGameObject, DamagedTarget.gameObject);
            DamagedTarget.OnDamaged(ActionDir);
        }
    }

    static public void OnAttacked_ByObj(CharacterBase DamagedTarget, float DamageAmount)
    {
        if (!DamagedTarget.ActorHasTag(DamagedTarget.Tag_Invincible)) // 무적이 아니면
        {
            //float DamageRate = Instigator.AbilitySpec.GetBulletDamageRate;
            //float GiveDamage = GenericCaculateDamage(Instigator, DamageAmount);

            TakeDamage(DamagedTarget, DamageAmount);

            DamagedTarget.OnDamaged_ByObj();
        }
    }
    #endregion

    #region Caculating Origin Damage
    public static float GenericCaculateDamage(CharacterBase Instigator, float AbilityDamageRate)
    {
        float DamageAmount = 0;

        DamageAmount = Instigator.CommonSpec.GetFinalATK * (AbilityDamageRate / 100);
        // 추가 공격력이나 스킬 피해량 변동 사항이 생길 시 추가작성

        return DamageAmount;
    }
    #endregion

    #region Damage Calculator
    static public void TakeDamage(CharacterBase DamagedTarget, float DamageAmount)
    {
        FloatingDamagePool.CallFloatingDamage(DamagedTarget.transform, DamageAmount);

        DamagedTarget.CommonSpec.DamageTaken(DamageAmount);
    }

    public static void TakeDamageToTarget(CharacterBase Instigator, CharacterBase Target, float OriginDamage)
    {
        if (!IsValid(Instigator))
        {
            Debug.LogError("Instigator is Invalid");
            return;
        }
        if (!IsValid(Target))
        {
            Debug.LogError("Target is Invalid");
            return;
        }

        if (Instigator == Target)
            return;

        if (Instigator.ActorHasTag(Instigator.Tag_PerfectDodge)) // 안전용
            return;

        if (Instigator.ActorHasTag(Instigator.Tag_Invincible)) // 안전용
            return;


        bool IsCritical = false;
        DamageType CallType = DamageType.Default;

        float EventRate = Random.value * 100;

        float DamageAmount = OriginDamage;

        if (Instigator.CommonSpec.GetCriticalRate > EventRate)
        {
            Debug.Log("Critical!");
            IsCritical = true;
            DamageAmount += OriginDamage * (Instigator.CommonSpec.GetCriticalDamage / 100);
        }

        // 임시 피해 경감
        DamageAmount /= Target.CommonSpec.GetDEF;
        DamageAmount = Mathf.Clamp(DamageAmount, 1, 99999);
        // 이후 맥스 대미지 설정
        if(IsCritical)
        {
            CallType = DamageType.Critical;
        }

        FloatingDamagePool.CallFloatingDamage(CallType, Target.transform, DamageAmount);

        Target.CommonSpec.DamageTaken(DamageAmount);
        // Target.OnDamaged(Vector2.zero);
    }

    public static void TakeDamageToSelf(CharacterBase Instigator, float OriginDamage)
    {
        Instigator.CommonSpec.DamageTaken(OriginDamage);
        Instigator.OnDamaged(Vector2.zero);
    }
    #endregion

    #region Heal Calculator
    public static void TakeHealToTarget(CharacterBase Instigator, CharacterBase Target, float OriginHealAmount, EHealType HealType)
    {
        if (!IsValid(Instigator) && !IsValid(Target))
        {
            Debug.LogError("Instigator or Target is Invalid");
            return;
        }

        if (Instigator == Target)
            return;

        float HealAmount = OriginHealAmount * Instigator.CommonSpec.GetRecoveryRate;

        // 이후 회복량 증감 조건의 추가

        // 힐 표시
        FloatingDamagePool.CallFloatingDamage(DamageType.Heal, Target.transform, HealAmount);

        if (HealType == EHealType.HPType)
        {
            Target.CommonSpec.HealTaken(HealAmount, HealType);
        }
        else if (HealType == EHealType.MPType)
        {
            Target.CommonSpec.HealTaken(HealAmount, HealType);
        }
    }

    public static void TakeHealToSelf(CharacterBase Instigator, float OriginHealAmount, EHealType HealType)
    {
        if(HealType == EHealType.HPType)
        {
            Instigator.CommonSpec.HealTaken(OriginHealAmount, HealType);
        }
        else if (HealType == EHealType.MPType)
        {
            Instigator.CommonSpec.HealTaken(OriginHealAmount, HealType);
        }
    }
    #endregion

    #endregion

    #region Sound Utility

    public static float GetVolumeByType(EVolumeType VolumeType)
    {
        switch(VolumeType)
        {
            case EVolumeType.BGM:
                return SoundManager.Instance.GetBgmVolume();

            case EVolumeType.Voice:
                return SoundManager.Instance.GetVoiceVolume();

            case EVolumeType.Effect:
                return SoundManager.Instance.GetEffectVolume();

            case EVolumeType.System:
                return SoundManager.Instance.GetSystemVolume();

            default:
                return 0f;
        }
    }

    public static void PlaySoundAtCharacter(AudioClip Sound, EVolumeType VolumeType, CharacterBase SoundInstigator)
    {
        if (!IsValid(Sound))
        {
            Debug.LogWarning("SoundLibrary: AudioClip is null.");
            return;
        }

        if(!IsValid(SoundInstigator))
        {
            Debug.LogWarning("Sound Instigator is Invalid!");
            return;
        }

        float Volume = GetVolumeByType(VolumeType);

        AudioSource AudioToPlay = SoundInstigator.AudioComp;

        AudioToPlay.PlayOneShot(Sound, Volume);
    }

    public static void PlaySound2D(AudioClip Sound, EVolumeType VolumeType, GameObject SoundInstigator)
    {
        if (!IsValid(Sound))
        {
            Debug.LogWarning("SoundLibrary: AudioClip is null.");
            return;
        }

        float Volume = GetVolumeByType(VolumeType);

        AudioSource AudioToPlay = SoundInstigator.GetComponent<AudioSource>();

        AudioToPlay.PlayOneShot(Sound, Volume);
    }

    public static void PlaySound2D(AudioClip Sound, EVolumeType VolumeType)
    {
        if (!IsValid(Sound))
        {
            Debug.LogWarning("SoundLibrary: AudioClip is null.");
            return;
        }

        float Volume = GetVolumeByType(VolumeType);

        AudioSource AudioToPlay;

        switch (VolumeType)
        {
            case EVolumeType.System:
                AudioToPlay = SoundManager.Instance.SystemPlayer;
                AudioToPlay.PlayOneShot(Sound, Volume);
                break;

            case EVolumeType.BGM:
                AudioToPlay = SoundManager.Instance.BGMPlayer;
                AudioToPlay.PlayOneShot(Sound, Volume);
                break;

            case EVolumeType.Effect:
                AudioToPlay = SoundManager.Instance.EffectPlayer;
                AudioToPlay.PlayOneShot(Sound, Volume);
                break;
        }
    }

    #endregion

    #region AI Task
    public static void MoveTo(CharacterBase ActionTarget, Vector2 MovePosition, float Tolerance, float MoveSpeed)
    {
        if(!IsValid(ActionTarget))
        {
            Debug.LogError("Move To Task: Action Target Is Null");

            return;
        }

        Rigidbody2D rb = ActionTarget.rb;

        Vector2 CurPos = rb.position;
        Vector2 Dir = (MovePosition - CurPos).normalized;

        if(Tolerance > 0)
        {
            if(GetDistance_A_to_B(ActionTarget.transform.position, MovePosition) <= Tolerance)
            {
                StopMove(ActionTarget);
                return;
            }
        }

        ActionTarget.MoveVec = new Vector2(Dir.x * MoveSpeed, rb.linearVelocityY);
        rb.linearVelocity = ActionTarget.MoveVec;
    }

    public static void StopMove(CharacterBase ActionTarget)
    {
        if (!IsValid(ActionTarget))
        {
            Debug.LogError("Move To Task: Action Target Is Null");

            return;
        }

        Rigidbody2D rb = ActionTarget.rb;

        ActionTarget.MoveVec.x = 0;
        rb.linearVelocityX = 0;

        ActionTarget.UsingStateMachine.ChangeState(ActionTarget._IdleState);
    }
    #endregion

}
