using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UIHelper : MonoSingleton<UIHelper>
{
    [SerializeField] private RectTransform _equipContextMenu;
    private bool _isContextOpen = false;
    [SerializeField] private ItemTooltipUI _itemItemTooltip;
    public ItemTooltipUI ItemTooltip => _itemItemTooltip;
    private ItemSlotUI _itemTooltipTargetSlot = null;

    [SerializeField] private StatTooltipUI _statTooltip;
    public StatTooltipUI StatTooltip => _statTooltip;

    [SerializeField] private CraftWindowUI _craftWindowUI;
    public CraftWindowUI CraftWindow => _craftWindowUI;

    [SerializeField] private SkillTooltipUI _skillTooltipUI;
    public SkillTooltipUI SkillTooltip => _skillTooltipUI;

    private void Awake()
    {
        Button equipBtn = _equipContextMenu.transform.Find("EquipButton").GetComponent<Button>();
        equipBtn.onClick.AddListener(()=>HandleEquipItemBtn());
        Button trashBtn = _equipContextMenu.transform.Find("TrashButton").GetComponent<Button>();
        trashBtn.onClick.AddListener(()=>HandleTrashItemBtn());
        
        _equipContextMenu.gameObject.SetActive(false);
        //시작하면 감춰두고.
        _itemItemTooltip.HideTooltip();
        _statTooltip.HideStatTooltip();
        _skillTooltipUI.HideTooltip();
    }
    

    public void OpenEquipContextMenu(Vector2 mousePosition, ItemSlotUI slot)
    {
        _isContextOpen = true;
        _itemTooltipTargetSlot = slot;
        
        _equipContextMenu.DOKill();
        _equipContextMenu.localScale = new Vector3(0, 0, 1f);
        _equipContextMenu.gameObject.SetActive(true);
        
        _equipContextMenu.anchoredPosition = mousePosition;
        _equipContextMenu.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).SetUpdate(true);
        
    }

    public void CloseEquipContextMenu()
    {
        if (!_isContextOpen) return;
        _isContextOpen = false;
        _itemTooltipTargetSlot = null;
        _equipContextMenu.DOKill();
        _equipContextMenu.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>
        {
            _equipContextMenu.gameObject.SetActive(false);
        });
    }

    private void HandleEquipItemBtn()
    {
        if (_itemTooltipTargetSlot == null) return;
        Inventory.Instance.EquipItem(_itemTooltipTargetSlot.item.data);
        CloseEquipContextMenu();
    }

    private void HandleTrashItemBtn()
    {
        if (_itemTooltipTargetSlot == null) return;
        Inventory.Instance.RemoveItem(_itemTooltipTargetSlot.item.data, 1);
        CloseEquipContextMenu();
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && _isContextOpen)
        {
            CloseEquipContextMenu();
        }
    }
}
