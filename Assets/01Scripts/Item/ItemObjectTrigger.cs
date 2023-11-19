using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{
    private ItemObject _itemObject;

    private void Awake()
    {
        _itemObject = GetComponentInParent<ItemObject>(); //부모에 있는거 가져오고.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            _itemObject.PickUpItem();
        }
    }

}
