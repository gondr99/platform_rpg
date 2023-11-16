using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    protected float _cooldownTimer;
    protected Player _player;

    protected virtual void Start()
    {
        _player = GameManager.Instance.Player;
    }

    protected virtual void Update()
    {
        _cooldownTimer -= Time.deltaTime;
    }

    public virtual bool AttemptUseSkill()
    {
        if (_cooldownTimer < 0)
        {
            _cooldownTimer = _cooldown;
            UseSkill(); //스킬을 사용하고
            return true;
        }
        Debug.Log("Skill cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
        
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
