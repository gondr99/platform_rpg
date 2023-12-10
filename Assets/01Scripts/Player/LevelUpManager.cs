
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelUpManager : MonoSingleton<LevelUpManager>, ISaveManager
{
    //레벨업당 1의 스킬포인트 && 5의 능력치 포인트
    public event Action<int> SkillPointChanged;
    public event Action<int> StatPointChanged;
    public event Action ExpChanged;
    private int _statPoint;
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
    public int StatPoint
    {
        get => _statPoint;
        private set
        {
            _statPoint = value;
            StatPointChanged?.Invoke(_statPoint);
        }
    }

    public int level = 1;
    public int nextExpPoint = 1000;
    [SerializeField]private int _currentExp = 0;

    public bool CanSpendSkillPoint()
    {
        if (SkillPoint <= 0) return false;
        
        SkillPoint -= 1;
        return true;
    }

    public float GetNormalizedExp()
    {
        if (nextExpPoint <= 0) return 0;
        return (float)_currentExp / nextExpPoint;
    }
    
    public void AddExp(int exp)
    {
        _currentExp += exp;
        ExpChanged?.Invoke();
        if (_currentExp >= nextExpPoint)
        {
            LevelUpProcess();
        }
    }

    private void LevelUpProcess()
    {
        _currentExp -= nextExpPoint;
        level += 1;
        SkillPoint += 1;
        StatPoint += 5;
        ExpChanged?.Invoke();
    }

    //디버그용 코드들
    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddExp(500);
        }
    }

    public void LoadData(GameData data)
    {
        _currentExp = data.exp;
        level = data.level;
        SkillPoint = data.skillPoint;
        StatPoint = data.statPoint;
        SetNextExpByLevel(level);
    }

    public void SaveData(ref GameData data)
    {
        data.exp = _currentExp;
        data.level = level;
        data.skillPoint = SkillPoint;
        data.statPoint = StatPoint;
    }

    private void SetNextExpByLevel(int level)
    {
        //레벨당 테이블 표 필요.
        nextExpPoint = 100;
    }
}
