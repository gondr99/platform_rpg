using System;
using TMPro;
using UnityEngine;

public class UIStatSlot : MonoBehaviour
{
    [SerializeField] private StatType _statType;
    [SerializeField] private string _statName;
    
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _statValueText;
    
    private void OnValidate()
    {
        gameObject.name = $"Stat - {_statType.ToString()}";

        if (!string.IsNullOrEmpty(_statName))
        {
            _statNameText.text = _statName;
        }
    }

    private void Start()
    {
        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        PlayerStat playerStat = GameManager.Instance.Player.Stat as PlayerStat;

        if (playerStat != null)
        {
            _statValueText.text = playerStat.GetStatByType(_statType).GetValue().ToString();
        }
    }
}
