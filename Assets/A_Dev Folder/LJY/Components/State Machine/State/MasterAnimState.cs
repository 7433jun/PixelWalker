using Unity.VisualScripting;
using UnityEngine;

public class MasterAnimState
{
    #region Owner Data
    protected CharacterBase OwningCharacter;
    protected PlayerController OwningPCont;
    #endregion

    protected MasterStateMachine UsedStateMachine;

    protected string AnimBoolName;

   // public float StateTimer { get; private set; } // ���� ���� �ð�

    #region Special Ability
    protected float DodgeEffectDuration;
    protected float PerfectDodgeTime;
    #endregion

    public MasterAnimState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName)
    {
        OwningCharacter = OwnedChara;
        UsedStateMachine = _UsedStateMachine;
        AnimBoolName = _AnimBoolName;

        if (OwningCharacter.CompareTag("Player"))
        {
            if(OwningPCont == null)
            {
                // ��Ʈ�ѷ� ������ �ȵ� ���¸� ��Ʈ�ѷ� ����
                OwningPCont = GetPlayerController();
            }
        }
    }

    public virtual void EnterState()
    {
        OwningCharacter.Anim.SetBool(AnimBoolName, true);

    }

    public virtual void Update()
    {
        // ToDo
        /* ������Ʈ�� ���� �ؾ��� ���� ���� */

        //if (StateTimer > 0)
            //StateTimer -= Time.deltaTime;

        OwningCharacter.Anim.SetFloat("yVelocity", GetOwnerRigidBody2D().linearVelocityY);
    }

    public virtual void ExitState()
    {
        if(!OwningCharacter.IsDead) // ������� ����
        {
            //if (StateTimer >= 0)
            //StateTimer = 0;

            if(OwningCharacter.IsBusy)
            {
                // ������ ���� Ż�� �߻��� ���� ��ġ
                OwningCharacter.IsBusy = false;
            }

            OwningCharacter.Anim.SetBool(AnimBoolName, false);
        }
    }

    #region Utility
    protected Rigidbody2D GetOwnerRigidBody2D()
    {
        if(!UtilityLibrary.IsValid(OwningCharacter))
        {
            Debug.LogError("Owning Character is Invalid!");

            return null;
        }

        return OwningCharacter.rb; 
    }

    protected Animator GetOwningCharacterAnimator()
    {
        return OwningCharacter.Anim;
    }

    protected void PlaySubAnim(string AnimIntegerName, int HasSubAnimsNum, string AnimBoolName, ref float ResetTimer)
    {
        int RandomNum = Random.Range(0, HasSubAnimsNum);

        GetOwningCharacterAnimator().SetFloat(AnimIntegerName, RandomNum);
        GetOwningCharacterAnimator().SetBool(AnimBoolName, true);

        ResetTimer = 0;
    }
    #endregion

    #region Player Data
    protected PlayerBase GetPlayerCharacter()
    {
        PlayerBase Player = OwningCharacter as PlayerBase;

        return Player;
    }

    protected PlayerController GetPlayerController()
    {
        return GetPlayerCharacter().PCont;
    }

    protected bool IsPlayer()
    {
        return OwningCharacter.CompareTag("Player");
    }

    #endregion

    #region Enemy Data
    protected EnemyBase GetEnemyCharacter()
    {
        EnemyBase Enemy = OwningCharacter as EnemyBase;

        return Enemy;
    }

    protected bool IsEnemyMob()
    {
        // ���̸鼭 ������ �ƴ� ��
        return OwningCharacter.CompareTag("Enemy") && !OwningCharacter.ActorHasTag(OwningCharacter.Tag_Boss);
    }

    protected MobBase GetMobCharacter()
    {
        MobBase Enemy = OwningCharacter as MobBase;

        return Enemy;
    }

    protected BossBase GetBossCharacter()
    {
        BossBase Boss = OwningCharacter as BossBase;

        return Boss;
    }

    protected bool IsBoss()
    {
        return OwningCharacter.CompareTag("Enemy") && OwningCharacter.ActorHasTag(OwningCharacter.Tag_Boss);
    }

    protected bool IsMinotaur()
    {
        return IsBoss() && OwningCharacter.CommonSpec.GetCharacterName == "Minotaur";
    }

    protected Minotaur GetMinotaur()
    {
        Minotaur Mino = OwningCharacter as Minotaur;

        return Mino;
    }
    #endregion

    /*
    protected AbilityManager GetAbilityManagerToOwner()
    {
        return OwningCharacter.CoolDownData;
    }
    */
}
