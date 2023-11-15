using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "SO/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    public event Action AttackEvent;
    public event Action JumpEvent;
    public event Action DashEvent;
    public event Action CounterAttackEvent;
    public event Action UltiSkillEvent;
    public event Action CrystalSkillEvent;
    public event Action<bool> ThrowAimEvent;
    public Vector2 AimPosition { get; private set; }
    public float xInput { get; private set; }
    public float yInput { get; private set; }

    private Controls _controls;
    private Controls.IPlayerActions _playerActionsImplementation;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        
        _controls.Player.Enable();
    }

    public void OnXMovement(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<float>();
    }
    
    public void OnYMovement(InputAction.CallbackContext context)
    {
        yInput = context.ReadValue<float>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AttackEvent?.Invoke();
        }
    }

    public void OnCounterAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CounterAttackEvent?.Invoke();
        }
    }

    public void OnThrowAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ThrowAimEvent?.Invoke(true);
        }else if (context.canceled)
        {
            ThrowAimEvent?.Invoke(false);
        }
    }

    public void OnMouseAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }

    public void OnUltiSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UltiSkillEvent?.Invoke();
        }
    }

    public void OnCrystalSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CrystalSkillEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpEvent?.Invoke();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DashEvent?.Invoke();
        }
    }

    
}
