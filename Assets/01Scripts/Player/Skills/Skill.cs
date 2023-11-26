
using System;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public bool skillEnalbed = false;
    
    [SerializeField] protected float _cooldown;
    protected float _cooldownTimer;
    protected Player _player;

    [SerializeField] protected PlayerSkill _skillType; 
    
    [HideInInspector] public LayerMask whatIsEnemy;

    public event Action<float, float> OnCoolDown;
    
    protected virtual void Start()
    {
        _player = GameManager.Instance.Player;
        whatIsEnemy = _player.DamageCasterCompo.whatIsEnemy;
    }

    protected virtual void Update()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;

            if (_cooldownTimer <= 0)
            {
                _cooldownTimer = 0;
            }
            
            OnCoolDown?.Invoke(_cooldownTimer, _cooldown);
        }
    }

    public virtual bool AttemptUseSkill()
    {
        if (_cooldownTimer <= 0 && skillEnalbed)
        {
            _cooldownTimer = _cooldown;
            UseSkill(); //스킬을 사용하고
            return true;
        }
        Debug.Log("Skill cooldown or locked");
        return false;
    }

    public virtual void UseSkill()
    {
        //스킬을 쓸 때마다 해당 스킬을 썼음을 알려주는 피드백 필요.
        SkillManager.Instance.UseSkillFeedback(_skillType);
    }

    public virtual void UseSkillWithoutCooltimeAndEffect()
    {
        //자동으로 발생되는 스킬들을 이용하기 위해 만든 함수.
    }

    public virtual Transform FindClosestEnemy(Transform checkTransform, LayerMask whatIsEnemy, float radius)
    {
        Transform closestEnemy = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTransform.position, radius, whatIsEnemy);

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            float distanceToEnemy = Vector2.Distance(checkTransform.position, collider.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = collider.transform;
            }
        }

        return closestEnemy;
    }
}
