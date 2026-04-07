using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;
    public bool cutscene = false;

    void Start()
    {
        CurrentState.EnterState();

    }

    protected virtual void Update() {
        EState nextStateKey = CurrentState.GetNextState();

        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.Statekey))
            CurrentState.UpdateState();
        else if (!IsTransitioningState) 
            TransitionToState(nextStateKey);
    }

    public void TransitionToState(EState stateKey) {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CurrentState.OnTriggerEnter2D(other);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        CurrentState.OnTriggerStay2D(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        CurrentState.OnTriggerExit2D(other);
    }
}
