using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//장비에서 피격시 발동해야할 이펙트가 있다면 여기서 발동시킴.
public class PlayerEffectByHit : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public void HandleHitInvokeEffect()
    {
        
        //장착중인 아이템에서 피격시 발동 아이템이 있다면 발동.
        Inventory.Instance.equipSlots.equipments.ForEach(equip =>
        {
            var equipItemData = equip.data as ItemDataEquipment;
            equipItemData.ItemEffectByHit(_player.HealthCompo);
        });
    }
}
