
using System;
using UnityEngine.InputSystem;

public class LevelUpManager : MonoSingleton<LevelUpManager>
{
    //레벨업당 5의 스킬포인트
    public event Action<int> SkillPointChanged;

    private int _skillPoint;
    public int SkillPoint
    {
        get => _skillPoint;
        private set
        {
            _skillPoint = value;
            SkillPointChanged?.Invoke(_skillPoint);
        }
    }

    public int level = 1;
    public int nextExpPoint = 1000;
    private int _currentExp;

    public bool CanSpendSkillPoint()
    {
        if (SkillPoint <= 0) return false;
        
        SkillPoint -= 1;
        return true;
    }
    
    public void AddExp(int exp)
    {
        _currentExp += exp;
        if (_currentExp > nextExpPoint)
        {
            LevelUpProcess();
        }
    }

    private void LevelUpProcess()
    {
        _currentExp -= nextExpPoint;
        level += 1;
        SkillPoint += 5;
    }


    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddExp(500);
        }
    }
}
