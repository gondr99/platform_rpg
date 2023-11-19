using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private ItemObject _dropPrefab;
    [SerializeField] private ItemData _item;
    
    public void DropItem(Vector2 dropVelocity)
    {
        ItemObject newDrop = Instantiate(_dropPrefab, transform.position, Quaternion.identity);
        newDrop.SetupItem(_item, dropVelocity);
        
    }
}
