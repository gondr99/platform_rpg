using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class FlaskCooldownUI : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _cooldownImage;

    private Sprite _emptySprite;
    private bool _equippedFlask;
    private float _lastUsedTime;
    private float _cooldown;
    
    private void Start()
    {
        _emptySprite = _itemImage.sprite; //기본 스프라이트를 빈 스프라이트로 처리.
        _cooldownImage.fillAmount = 0;

        Inventory.Instance.OnFlaskCooldownEvent += HandleFlaskCooldown;
    }

    private void HandleFlaskCooldown( bool equipped,  float lastUsedTime, float cooldown)
    {
        _equippedFlask = equipped;
        if (_equippedFlask)
        {
            ItemDataEquipment flask = Inventory.Instance.GetEquipmentByType(EquipmentType.Flask);

            _cooldownImage.sprite = flask.icon;
            _itemImage.sprite = flask.icon;
            _lastUsedTime = lastUsedTime;
            _cooldown = cooldown;
        }
        else
        {
            _cooldownImage.sprite = _emptySprite;
            _itemImage.sprite = _emptySprite;
            _cooldownImage.fillAmount = 0;
        }
    }


    private void Update()
    {
        if (_equippedFlask && _lastUsedTime + _cooldown > Time.time)
        {
            _cooldownImage.fillAmount = (1 - (Time.time - _lastUsedTime) / +_cooldown);
        }
    }
}
