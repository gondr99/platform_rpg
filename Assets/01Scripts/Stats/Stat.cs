using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;

    public List<int> modifiers;
    public int GetValue()
    {
        int finalValue = _baseValue;
        foreach (int value in modifiers)
        {
            finalValue += value;
        }
        return finalValue;
    }

    public void AddModifier(int value)
    {
        modifiers.Add(value);
    }

    public void RemoveModifire(int value)
    {
        modifiers.Remove(value);
    }

    public void SetDefaultValue(int value)
    {
        _baseValue = value;
    }
}
