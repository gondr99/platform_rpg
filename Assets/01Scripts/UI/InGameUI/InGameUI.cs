using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Image _healthSlider;
    [SerializeField] private Image _expSlider; 
    
    private Health _playerHealth;
    

    private void Start()
    {
        _playerHealth = GameManager.Instance.Player.HealthCompo;
        _playerHealth.OnHit += HandleHitEvent;

        LevelUpManager.Instance.ExpChanged += HandleExpEvent;
        HandleExpEvent(); //최초 한번 실행
    }

    private void HandleHitEvent()
    {
        _healthSlider.fillAmount = _playerHealth.GetNormalizedHealth();
    }

    private void HandleExpEvent()
    {
        _expSlider.fillAmount = LevelUpManager.Instance.GetNormalizedExp();
    }
}
