using Project.Structs;
using System.Collections;
using System.Drawing;
using UnityEngine;
using Project.Enums;

public class Unity_Chan : PlayerBase
{
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();

        //PlayerSpec.DamageTaken(30);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Update()
    {
        base.Update();
        Debugger();
    }

    protected override IEnumerator BuffPackage01(float Duration)
    {
        Search_Effect(EEffectType.Buff_PowerUp);

        // Unitychan Sword Skill
        //Debug.Log("�ҵ� ��ų ����");
        PlayerSpec.SetAdditiveATK_ToMultifly(20);
        PlayerSpec.SetAdditiveMoveSpeed(0.2f);
        Anim.speed = 1.1f;

        yield return new WaitForSecondsRealtime(Duration);

        //Debug.Log("�ҵ� ��ų ����");
        PlayerSpec.SetAdditiveATK_ToMultifly(-20);
        PlayerSpec.SetAdditiveMoveSpeed(-0.2f);
        Anim.speed = 1;
    }

    void Debugger()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            //PlayerSpec.EnableRegenHP(true);
        }
        if (Input.GetKey(KeyCode.E))
        {
            //PlayerSpec.EnableDoubleJump(true);
        }
        if(Input.GetKey(KeyCode.T))
        {
            ApplyStatusEffectToSelf(EStatusEffect.Bleeding, 5, 1);
        }
    }
}
