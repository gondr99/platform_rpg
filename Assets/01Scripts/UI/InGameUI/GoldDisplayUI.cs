using TMPro;
using UnityEngine;

public class GoldDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldAmountText;

    [SerializeField] private int _currentGold;
    [SerializeField] private int _increaseAmount = 500;

    private int _currentUIValue = 0;

    private bool _changeImediately = false;
    private void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += HandleGoldChange;
        _changeImediately = true; //즉시 변환
        HandleGoldChange(CurrencyManager.Instance.Curreny); //맨 처음 한번은 실행
    }

    public void HandleGoldChange(int value)
    {
        _currentGold = value;
    }

    private void Update()
    {
        if (_changeImediately)
        {
            _currentUIValue = _currentGold;
            _goldAmountText.text = _currentUIValue.ToString();
            _changeImediately = false;
        }
        
        if (_currentUIValue < _currentGold)
        {
            _currentUIValue += Mathf.CeilToInt(_increaseAmount * Time.deltaTime);


            if (_currentUIValue >= _currentGold)
            {
                _currentUIValue = _currentGold;
            }
                
            _goldAmountText.text = _currentUIValue.ToString();
        }
    }
}
