using System;
using UnityEngine;

[Serializable]
public enum DamageCategory
{
    Noraml = 0,
    Critical = 1,
    Heal = 2,
    Debuff = 3,
}
public class DamageTextManager : MonoSingleton<DamageTextManager>
{
    public bool _popupDamageText;

    [Header("normal, critical, heal, debuff")]
    [ColorUsage(true, true)]
    [SerializeField] private Color[] _textColors;
    [SerializeField] private float[] _textSizes;
    
    public void PopupDamageText(Vector3 position, int number, DamageCategory category)
    {
        if(!_popupDamageText) return; //텍스트가 뜨기로 되어 있을 때만 띄운다.
        
        DamageText _damageText = PoolManager.Instance.Pop(PoolingType.DamageText) as DamageText;

        int idx = (int)category;
        _damageText.ShowDamageText(position, number, _textSizes[idx], _textColors[idx]);
    }
}
