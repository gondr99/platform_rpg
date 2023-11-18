using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private ItemData _itemData;

    private void OnValidate()
    {
        if(_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _itemData.icon;
        gameObject.name = $"ItemObject-[{_itemData.itemName}]";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Inventory.Instance.AddItem(_itemData);
            Destroy(gameObject);
        }
    }
}
