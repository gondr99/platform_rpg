using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Image _slider;
    
    private Health _playerHealth;

    private void Start()
    {
        _playerHealth = GameManager.Instance.Player.HealthCompo;
        _playerHealth.OnHit += HandleHitEvent;
    }

    private void HandleHitEvent()
    {
        _slider.fillAmount = _playerHealth.GetNormalizedHealth();
    }
}
