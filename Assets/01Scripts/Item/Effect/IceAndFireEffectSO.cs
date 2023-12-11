using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/IceAndFire")]
public class IceAndFireEffectSO : ItemEffectSO
{
    [SerializeField] private IceAndFireController _iceAndFirePrefab;
    [SerializeField] private float _velocity;
    public override void UseEffect()
    {
        CastIceAndFire(GameManager.Instance.Player);
    }

    public override bool ExecuteEffectByMelee(bool hitAttack)
    {
        if (activeByHit && !hitAttack) return false; 
        
        if (Random.Range(0, 100f) > effectChance)
        {
            return false;
        }

        Player player = GameManager.Instance.Player;
        if (player.currentComboCounter != 2) return false; //매 3타마다 발동
        
        CastIceAndFire(player);
        
        _lastMeleeEffectTime = Time.time;
        return true;
    }

    private void CastIceAndFire(Player player)
    {
        Vector3 spawnPosition = player.transform.position + new Vector3(2 * player.FacingDirection, 0);

        IceAndFireController instance = Instantiate(_iceAndFirePrefab, spawnPosition, Quaternion.identity);
        instance.FireToFront( new Vector2( player.FacingDirection * _velocity, 0), 2f );
        
    }
}
