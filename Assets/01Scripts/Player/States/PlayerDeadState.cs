

using System.Threading.Tasks;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _player.canStateChangeable = false; //상태 변경 불가능하도록 변경
        DelayStop();
    }

    private async void DelayStop()
    {
        await Task.Delay(1000); //1초후 정지.
        _player.StopImmediately(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }
}
