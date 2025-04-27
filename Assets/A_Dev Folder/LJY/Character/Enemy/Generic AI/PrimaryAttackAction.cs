using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Primary Attack", story: "Attack", category: "Action", id: "7a107b4aa73f9f3536d03ccc7ccced6d")]
public partial class PrimaryAttackAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;


    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

