using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Project.Enums;

public class EnemyBase : CharacterBase
{
    public EnemySpecData EnemySpec { get; private set; }

    #region Blackboard
    [Header("Debug Blackboard")]
    public CharacterBase AttackTarget;// { get; private set; }

    public bool InAttackRange;

    public float AttackRange;
    public float RememberTargetTime = 4f;
    public float MissingTime = 0;

    public Vector2 LastLookPoint;

    #endregion

    #region Generic
    public bool TargetIsBack()
    {
        if(AttackTarget)
        {
            // 타겟이 서치된 상황에서만
            if ((UtilityLibrary.GetDirection_A_to_B(this.gameObject, AttackTarget.gameObject).x > 0 && FacingDir == -1) || 
                (UtilityLibrary.GetDirection_A_to_B(this.gameObject, AttackTarget.gameObject).x < 0 && FacingDir == 1))
            {
                return true;
            }
        }
        return false;
    }

    protected bool TargetIsInRange() => UtilityLibrary.GetDistance_A_to_B(this, AttackTarget) <= AttackRange;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        EnemySpec = CommonSpec as EnemySpecData;
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

    }

    public void Enemy_ItemDrop()
    {
        EnemySpec.SpawnDropItem((Vector2)transform.position + new Vector2(0, 1));
    }
}
