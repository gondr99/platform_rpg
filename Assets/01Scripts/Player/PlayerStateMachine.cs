
using System.Collections.Generic;
using UnityEngine;
public enum StateEnum
{
    Idle,
    Move,
    Jump,
    Air,
    Dash,
    WallSlide,
    WallJump,
    PrimaryAttack,
    CounterAttack,
    AimSword,
    CatchSword,
}
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    public Dictionary<StateEnum, PlayerState> StateDictionary = new Dictionary<StateEnum, PlayerState>();

    public void Initialize(StateEnum startState)
    {
        
        CurrentState = StateDictionary[startState];
        CurrentState.Enter();
    }

    public void ChangeState(StateEnum newState)
    {
        CurrentState.Exit();
        CurrentState = StateDictionary[newState];
        CurrentState.Enter();
    }

    public void AddState(StateEnum state, PlayerState playerState)
    {
        StateDictionary.Add(state, playerState);
    }
}
