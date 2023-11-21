using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/IceAndFire")]
public class IceAndFireEffectSO : ItemEffectSO
{
    [SerializeField] private IceAndFireController _iceAndFirePrefab;
    [SerializeField] private float _velocity;
    public override void ExecuteEffectByMelee(bool hitAttack)
    {
        if (activeByHit && !hitAttack) return; 
        
        if (Random.Range(0, 100f) > effectChance)
        {
            return;
        }

        Player player = GameManager.Instance.Player;
        if (player.currentCompoCounter != 2) return; //매 3타마다 발동
        
        Vector3 spawnPosition = player.transform.position + new Vector3(2 * player.FacingDirection, 0);

        IceAndFireController instance = Instantiate(_iceAndFirePrefab, spawnPosition, Quaternion.identity);
        instance.FireToFront( new Vector2( player.FacingDirection * _velocity, 0), 2f );
        
    }

}
