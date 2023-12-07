using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float _flyTime = 0.25f;
    private bool _skillUsed;
    private float _startTimer;

    private float _originalGravityScale;
    private BlackholeSkill _skill;
    
    public PlayerBlackholeState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _player.PlayerInput.UltiSkillEvent += OnUltiSkillPressed;
        _skillUsed = false;
        // 타이머 기록 시작.
        _startTimer = _flyTime;
        _originalGravityScale = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0;
        _player.canStateChangeable = false; //상태를 변경하지 못하도록 하고

        _skill = _player.skill.GetSkill<BlackholeSkill>();

        _skill.SkillEffectEnd += OnSkillEffectEnd;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _startTimer -= Time.deltaTime;
        if (_startTimer > 0)
        {
            _player.SetVelocity(0, 15f, true);
        }

        if (_startTimer <= 0)
        {
            _player.StopImmediately(true);
            if (!_skillUsed)
            {
                _skill.BlackholeFieldOpen(_player.transform.position);
                _skillUsed = true; 
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        _player.PlayerInput.UltiSkillEvent -= OnUltiSkillPressed;
        _skill.SkillEffectEnd -= OnSkillEffectEnd;
        _rigidbody.gravityScale = _originalGravityScale;
    }

    //스킬 이펙트까지 모두 종료되면.
    private void OnSkillEffectEnd()
    {
        _player.canStateChangeable = true; //상태를 변경하지 못하도록 하고
        _stateMachine.ChangeState(StateEnum.Idle);
    }

    private void OnUltiSkillPressed()
    {
        if (_skillUsed)
        {
            _skill.ReleaseAttack(); //공격시작.
        }
    }
}
