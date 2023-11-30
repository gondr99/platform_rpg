using System;
using UnityEngine;

public class CurrencyManager : MonoSingleton<CurrencyManager>, ISaveManager
{
    public Action<int> OnCurrencyChanged;
    [SerializeField] private int _curreny = 0;

    public int Curreny
    {
        get => _curreny;
        private set
        {
            _curreny = value;
            OnCurrencyChanged?.Invoke(_curreny);
        }
    }
    public bool HasEnoughMoney(int price)
    {
        return Curreny >= price;
    }

    public void AddCurreny(int value)
    {
        Curreny += value;
    }
    
    public void LoadData(GameData data)
    {
        Curreny = data.curreny;
    }

    public void SaveData(ref GameData data)
    {
        data.curreny = Curreny;
    }
}
