using System;
using TMPro;
using UnityEngine;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    private void Start()
    {
        
    }

    public void ShowTooltip(ItemDataEquipment item)
    {
        if(item == null) return;
        _itemNameText.text = item.itemName;
        _itemTypeText.text = item.equipmentType.ToString();
        _itemDescriptionText.text = item.GetDescription();
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
