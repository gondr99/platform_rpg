using System;
using UnityEngine;

public class ItemObject : MonoBehaviour, IPIckable
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
    
    public void PickUp()
    {
        if (_itemData.itemType == ItemType.Equipment && !Inventory.Instance.CanAddItem())
        {
            _rigidbody.velocity = new Vector2(0, 7); //잠깐 위로 떠오르고
            return; //안줍는다.
        }
        
        AudioManager.Instance.PlaySFX(18, transform);
        
        Inventory.Instance.AddItem(_itemData);
        Destroy(gameObject);
    }
}
