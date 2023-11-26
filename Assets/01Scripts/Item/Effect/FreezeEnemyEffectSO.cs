using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/Freeze near Enemies")]
public class FreezeEnemyEffectSO : ItemEffectSO
{
    [Range(0, 3f)]
    public float effectRadius;
    public LayerMask whatIsEnemy;
    public float freezeDuration;
    [Range(0, 1f)]
    public float invokeHealthPercent = 0.1f;

    private List<Enemy> _targetList = new List<Enemy>();
    public override void UseEffect()
    {
        Debug.Log("쓰면 안되는데 써졌어 확인해라.");
    }

    public override bool ExecuteEffectByHit(Health health)
    {
        if (!base.ExecuteEffectByHit(health)) return false;
        if (health.GetNormalizedHealth() > invokeHealthPercent) return false; //체력이 일정퍼센트 이하일때만 발동.
        
        FillTargetList();
        Debug.Log("발동!");
        FreezeToTargets();
        _lastHitEffectTime = Time.time;
        return true;
    }
    
    private void FreezeToTargets()
    {
        Player player = GameManager.Instance.Player;
        foreach (Enemy enemy in _targetList)
        {
            enemy.FreezeTimerFor(freezeDuration);
            enemy.HealthCompo.SetAilment(Ailment.Chilled, freezeDuration, player.Stat.GetDotDamage(Ailment.Chilled));
        }
    }

    private void FillTargetList()
    {
        Player player = GameManager.Instance.Player;
        _targetList.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, effectRadius, whatIsEnemy);
        if (colliders.Length == 0) return; //아무것도 없다면 할게 없다.
        _targetList = colliders.Select(x => x.GetComponent<Enemy>()).ToList();
    }
}
