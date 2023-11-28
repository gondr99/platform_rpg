using UnityEngine;

public class CurrencyManager : MonoSingleton<CurrencyManager>, ISaveManager
{
    [SerializeField] private int _curreny = 0;

    public bool HasEnoughMoney(int price)
    {
        return _curreny >= price;
    }

    public int GetCurrency() => _curreny;
    
    
    public void LoadData(GameData data)
    {
        _curreny = data.curreny;
    }

    public void SaveData(ref GameData data)
    {
        data.curreny = _curreny;
    }
}
