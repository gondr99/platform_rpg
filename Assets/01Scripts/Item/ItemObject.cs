using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private ItemData _itemData;

    private void OnValidate()
    {
        if (_itemData == null) return;
        if(_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _itemData.icon;
        gameObject.name = $"ItemObject-[{_itemData.itemName}]";
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetupItem(ItemData itemData, Vector2 velocity)
    {
        _itemData = itemData;
        _rigidbody.velocity = velocity;
        _spriteRenderer.sprite = _itemData.icon;
    }
    
    public void PickUpItem()
    {
        Inventory.Instance.AddItem(_itemData);
        Destroy(gameObject);
    }
}
