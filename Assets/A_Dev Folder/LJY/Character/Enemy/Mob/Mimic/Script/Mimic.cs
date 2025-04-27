using UnityEngine;

public class Mimic : MobBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();
        if(IsBattle)
        {
            GenericMobAI();
        }
    }

    public bool IsBattle;
    public void Mimic_EndTransform()
    {
        Anim.SetBool("NonBattle", false);
        RemoveTag(Tag_Invincible);
        IsBattle = true;
        AttackTarget = UtilityLibrary.GetPlayerCharacterInGame();
    }
}
