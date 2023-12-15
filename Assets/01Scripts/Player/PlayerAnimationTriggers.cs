using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player _player;


    private void Awake()
    {
        _player = transform.parent.GetComponent<Player>();
    }

    private void AnimationTrigger()
    {
        _player.AnimationTrigger();
    }
    
    private void AttackTrigger()
    {
        _player.Attack();
    }

    //칼을 던지는 이벤트.
    private void ThrowSword()
    {
        SwordSkill skill = _player.skill.GetSkill<SwordSkill>();
        skill.CreateSword();
    }
}
