using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public delegate void SkillUpgrade(int currentCount);
public class SkillTreeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    [SerializeField] private string _skillName;
    [TextArea][SerializeField] private string _skillDescription;

    [SerializeField] private SkillTreeSlotUI[] _shouldBeUnlocked; //이 스킬에 이르기 위해 해제되어야 하는 스킬
    [SerializeField] private SkillTreeSlotUI[] _shouldBeLocked; //이 스킬에 이르기 위해 해제되면 안되는 스킬

    [SerializeField] private Color _lockedSkillColor;
    
    
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _upgradeCountText;
    public bool unlocked;

    public int maxUpgradeCount;
    private int _currentUpgradeCount;

    public event SkillUpgrade UpgradeEvent;
    
    private void OnValidate()
    {
        gameObject.name = $"SkillTreeSlotUI-[ {_skillName} ]";
    }

    private void Start()
    {
        _skillImage = GetComponent<Image>();
        _skillImage.color = _lockedSkillColor;
        GetComponent<Button>().onClick.AddListener(()=>UnlockSkillSlot());
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (unlocked)
        {
            _skillImage.color = Color.white;
        }
        _upgradeCountText.text = $"{_currentUpgradeCount.ToString()} / {maxUpgradeCount.ToString()}";
    }

    public void UnlockSkillSlot()
    {
        for (int i = 0; i < _shouldBeUnlocked.Length; ++i)
        {
            if (_shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Can not unlock this skill");
                return;
            }
        }

        for (int i = 0; i < _shouldBeLocked.Length; ++i)
        {
            if (_shouldBeLocked[i].unlocked)
            {
                Debug.Log("Can not unlock this skill");
                return;
            }
        }

        if (_currentUpgradeCount == maxUpgradeCount)
        {
            Debug.Log("Can not upgrade this skill");
            return;
        }
        
        
        if (LevelUpManager.Instance.CanSpendSkillPoint())
        {
            if (unlocked == false)
            {
                unlocked = true;
            }
            
            ++_currentUpgradeCount;
            UpgradeEvent?.Invoke(_currentUpgradeCount); //현재 업그레이드 상태 전송
            UpdateUI();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHelper.Instance.SkillTooltip.ShowTooltip(_skillName, _skillDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHelper.Instance.SkillTooltip.HideTooltip();
    }

    public void LoadData(GameData data)
    {
        if (data.skillTree.TryGetValue(_skillName, out int value))
        {
            unlocked = value > 0;
            _currentUpgradeCount = value;

            if (unlocked)
            {
                UpdateUI();
                UpgradeEvent?.Invoke(_currentUpgradeCount);
            }
        }
        
    }

    public void SaveData(ref GameData data)
    {
        if (data.skillTree.TryGetValue(_skillName, out int value))
        {
            data.skillTree.Remove(_skillName);
        }
        
        data.skillTree.Add(_skillName, _currentUpgradeCount);
    }
}
