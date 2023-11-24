using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;
    [SerializeField] private Image _itemIcon;

    [SerializeField] private Image[] _materialsImages;

    [SerializeField] private Button _craftButton;

    private void Start()
    {
        ClearCraftWindow();
    }

    public void SetUpCraftWindow(ItemDataEquipment data)
    {
        
        _craftButton.onClick.RemoveAllListeners();
        
        //모든 슬롯 지우고
        for (int i = 0; i < _materialsImages.Length; ++i)
        {
            _materialsImages[i].color = Color.clear;
            _materialsImages[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        //만약 슬롯보다 더 많으면? 
        for (int i = 0; i < data.craftingMaterials.Count; ++i)
        {
            _materialsImages[i].sprite = data.craftingMaterials[i].data.icon;
            _materialsImages[i].color = Color.white;

            var tmp = _materialsImages[i].GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = data.craftingMaterials[i].stackSize.ToString();
            tmp.color = Color.white;
        }

        _itemIcon.sprite = data.icon;
        _itemName.text = data.itemName;
        _itemDescription.text = data.GetDescription();
        
        
        _craftButton.onClick.AddListener(() =>
        {
            Inventory.Instance.CanCraft(data, data.craftingMaterials);
        });
    }

    public void ClearCraftWindow()
    {
        //모든 슬롯 지우고
        for (int i = 0; i < _materialsImages.Length; ++i)
        {
            _materialsImages[i].color = Color.clear;
            _materialsImages[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;

            _itemIcon.sprite = null;
            _itemName.text = "제작을 원하는 아이템을 선택하세요.";
            _itemDescription.text = "아이템 제작";
        }
    }
}
