using UnityEngine;

public class DodgeState : MasterAnimState
{
    float OriginDuration;
    float DodgeDuration;


    public DodgeState(CharacterBase OwnedChara, MasterStateMachine _UsedStateMachine, string _AnimBoolName) : base(OwnedChara, _UsedStateMachine, _AnimBoolName)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        //Debug.Log("Dodge");
        OriginDuration = OwningCharacter.AbilitySpec.GetDodgeDuration;
        DodgeDuration = 0;

        OwningCharacter.AddTag(OwningCharacter.Tag_Invincible);
        OwningCharacter.AddTag(OwningCharacter.Tag_PerfectDodge);

        if(IsPlayer())
        {
            OwningCharacter.AfterImageEffector.StartAfterImageEffect();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        //Debug.Log("Dodge End");
        if (OwningCharacter.ActorHasTag(OwningCharacter.Tag_Invincible))
        {
            OwningCharacter.RemoveTag(OwningCharacter.Tag_Invincible);
        }

        if (OwningCharacter.ActorHasTag(OwningCharacter.Tag_PerfectDodge))
        {
            OwningCharacter.RemoveTag(OwningCharacter.Tag_PerfectDodge);
        }

    }

    public override void Update()
    {
        base.Update();
        
        if (DodgeDuration < OriginDuration)
        {
            OwningCharacter.SetVelocity(OwningCharacter.FacingDir * OwningCharacter.CommonSpec.GetDashSpeed * 2, OwningCharacter.rb.linearVelocityY);
            DodgeDuration += Time.deltaTime;

            if(OriginDuration / 3 <= DodgeDuration)
            {
                if(OwningCharacter.ActorHasTag(OwningCharacter.Tag_PerfectDodge))
                {
                    OwningCharacter.RemoveTag(OwningCharacter.Tag_PerfectDodge);
                }
            }
        }
        else
        {
            OwningCharacter.UsingStateMachine.ChangeState(OwningCharacter._IdleState);
        }
    }
}
