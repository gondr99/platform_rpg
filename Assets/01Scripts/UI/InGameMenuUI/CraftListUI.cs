using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftListUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform _craftSlotParent;
    [SerializeField] private CraftSlotUI _craftSlotPrefab;

    [SerializeField] private List<ItemDataEquipment> _craftEquipment;

    public void SetUpCraftList()
    {
        //모든 슬롯 정리하고 새로 만들기. (이걸 람다로 하면 안된다.)- why??
        for (int i = 0; i < _craftSlotParent.childCount; ++i)
        {
            Destroy(_craftSlotParent.GetChild(i).gameObject);
        }

        // //제작 가능한 장비들에 대해 리스트를 만든다.
        _craftEquipment.ForEach(craftItem =>
        {
            CraftSlotUI newSlot = Instantiate(_craftSlotPrefab, _craftSlotParent);
            newSlot.SetUpCraftSlot(craftItem);
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetUpCraftList();
        SetupDefaultCraftWindow();
    }

    public void SetupDefaultCraftWindow()
    {
        if (_craftEquipment.Count > 0)
        {
            UIHelper.Instance.CraftWindow.SetUpCraftWindow(_craftEquipment[0]);
        }
        else
        {
            UIHelper.Instance.CraftWindow.ClearCraftWindow();
        }
    }
}
