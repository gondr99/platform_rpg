using UnityEngine;
using UnityEngine.EventSystems;

public class MenuWindowUI : MonoBehaviour, IPointerDownHandler
{
    
    public void OnPointerDown(PointerEventData eventData)
    {
        UIHelper.Instance.CloseEquipContextMenu();
    }
}
