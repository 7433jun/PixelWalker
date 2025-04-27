using UnityEngine;

public class MasterStateMachine
{
    public MasterAnimState CurState { get; private set; }

    public void StateInitialize(MasterAnimState StartState)
    {
        CurState = StartState;
        CurState.EnterState();
    }

    public void ChangeState(MasterAnimState NewState)
    {
        CurState.ExitState();
        CurState = NewState;
        CurState.EnterState();
    }
}
