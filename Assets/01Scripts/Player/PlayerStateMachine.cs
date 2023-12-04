
using System.Collections.Generic;
using UnityEngine;
public enum StateEnum
{
    Idle,
    Move,
    Jump,
    Fall,
    Dash,
    WallSlide,
    WallJump,
    PrimaryAttack,
    CounterAttack,
    AimSword,
    CatchSword,
    Blackhole,
    Dead,
}
public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    public Dictionary<StateEnum, PlayerState> StateDictionary = new Dictionary<StateEnum, PlayerState>();

    private Player _player;
    public void Initialize(StateEnum startState, Player player)
    {
        _player = player;
        CurrentState = StateDictionary[startState];
        CurrentState.Enter();
    }

    public void ChangeState(StateEnum newState)
    {
        if (!_player.canStateChangeable) return;
        
        CurrentState.Exit();
        CurrentState = StateDictionary[newState];
        CurrentState.Enter();
    }

    public void AddState(StateEnum state, PlayerState playerState)
    {
        StateDictionary.Add(state, playerState);
    }
}
