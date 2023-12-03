using System;
using DG.Tweening;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    
    public static Action<string> CheckPointActiveEvent;
    
    private Animator _animator;
    public string checkpointID;
    public bool isActivated = false;

    private InteractionFillUI _interactionUI;
    
    private readonly int _hashActive = Animator.StringToHash("active");
    private Player _interactionPlayer = null;
    [SerializeField] private float _chargingTime = 1.3f;
    private float _currentChargingTime = 0f;
    private bool _isCharging = false;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _interactionUI = transform.Find("HoldKeyUI").GetComponent<InteractionFillUI>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateID()
    {
        checkpointID = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
        if (other.TryGetComponent<Player>(out Player player))
        {
            //나중에 여기서 boolean값 주고 활성화 키 넣고 활성화키 누르면 세이브 포인트 활성화 되도록한다.
            _interactionPlayer = player;
            player.PlayerInput.InteractionEvent += HandlePlayerInteractionInput;
            _interactionUI.SetActiveState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isActivated) return; //이미 활성화된 애들은 굳이 안해도 된다.
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.PlayerInput.InteractionEvent -= HandlePlayerInteractionInput;
            _interactionUI.SetActiveState(false);
            _isCharging = false; //나가면 바로 취소
            _interactionPlayer = null;
        }
    }

    private void HandlePlayerInteractionInput(bool value)
    {
        _isCharging = value;
    }

    private void Update()
    {
        if (_isCharging)
        {
            _currentChargingTime = Mathf.Clamp(_currentChargingTime + Time.deltaTime, 0, _chargingTime);
            _interactionUI.SetNormalizedGauge(_currentChargingTime / _chargingTime);

            if (_currentChargingTime >= _chargingTime)
            {
                ActiveCheckPoint();
            }
        }
        else if(_currentChargingTime > 0)
        {
            _currentChargingTime = Mathf.Clamp(_currentChargingTime - Time.deltaTime, 0, _chargingTime);
            _interactionUI.SetNormalizedGauge(_currentChargingTime / _chargingTime);
        }
        
    }

    public void ActiveCheckPoint()
    {
        AudioManager.Instance.PlaySFX(5, transform);
        _interactionUI.SetActiveState(false);
        _isCharging = false; 
        isActivated = true;
        _animator.SetBool(_hashActive, true);
        CheckPointActiveEvent?.Invoke(checkpointID); //이 체크포인트가 활성화 되었음을 알림.
    }
}
