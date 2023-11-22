using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UIContextManager : MonoSingleton<UIContextManager>
{
    [SerializeField] private RectTransform _equipContextMenu;
    private bool _isContextOpen = false;
    [SerializeField] private ItemTooltipUI _itemTooltip;
    public ItemTooltipUI Tooltip => _itemTooltip;
    
    private ItemSlotUI _targetSlot = null;

    private void Awake()
    {
        Button equipBtn = _equipContextMenu.transform.Find("EquipButton").GetComponent<Button>();
        equipBtn.onClick.AddListener(()=>HandleEquipItemBtn());
        Button trashBtn = _equipContextMenu.transform.Find("TrashButton").GetComponent<Button>();
        trashBtn.onClick.AddListener(()=>HandleTrashItemBtn());
        
        _equipContextMenu.gameObject.SetActive(false);
        _itemTooltip.HideTooltip();
    }
    

    public void OpenEquipContextMenu(Vector2 mousePosition, ItemSlotUI slot)
    {
        _isContextOpen = true;
        _targetSlot = slot;
        
        _equipContextMenu.DOKill();
        _equipContextMenu.localScale = new Vector3(0, 0, 1f);
        _equipContextMenu.gameObject.SetActive(true);
        
        _equipContextMenu.anchoredPosition = mousePosition;
        _equipContextMenu.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        
    }

    public void CloseEquipContextMenu()
    {
        if (!_isContextOpen) return;
        _isContextOpen = false;
        _targetSlot = null;
        _equipContextMenu.DOKill();
        _equipContextMenu.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _equipContextMenu.gameObject.SetActive(false);
        });
    }

    private void HandleEquipItemBtn()
    {
        if (_targetSlot == null) return;
        Inventory.Instance.EquipItem(_targetSlot.item.data);
        CloseEquipContextMenu();
    }

    private void HandleTrashItemBtn()
    {
        if (_targetSlot == null) return;
        Inventory.Instance.RemoveItem(_targetSlot.item.data, 1);
        CloseEquipContextMenu();
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CloseEquipContextMenu();
        }
    }
}
