using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private ItemData _itemData;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _itemData.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            Debug.Log($"pick up item : {_itemData.itemName}");
            Destroy(gameObject);
        }
    }
}
