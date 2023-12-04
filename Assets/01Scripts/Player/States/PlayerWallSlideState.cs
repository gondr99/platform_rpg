using UnityEngine;

public class PlayerWallSlideState : PlayerState
{

    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _player.PlayerInput.JumpEvent += HandleJump;
    }

    private void HandleJump()
    {
        _stateMachine.ChangeState(StateEnum.WallJump);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        float xInput = _player.PlayerInput.xInput;
        float yInput = _player.PlayerInput.yInput;

        if (yInput < 0)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);    
        }
        else
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y * 0.7f);    
        }

        //다른방향으로 키를 눌렀다면.
        if (Mathf.Abs(xInput + _player.FacingDirection) < 0.5f) 
        {
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    
        //땅에 닿았다면 취소.
        if (_player.IsGroundDetected() || !_player.IsWallDetected())
        {
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    }

    public override void Exit()
    {
        _player.PlayerInput.JumpEvent -= HandleJump;
        base.Exit();
    }
}
