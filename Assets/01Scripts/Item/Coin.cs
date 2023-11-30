using System;
using UnityEngine;

public class Coin : MonoBehaviour, IPIckable
{
    private Rigidbody2D _rigidbody;
    private int _value;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetValue(int value)
    {
        _value = value;
    }

    public void AddForce(Vector2 force)
    {
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
    }
    
    public void PickUp()
    {
        CurrencyManager.Instance.AddCurreny(_value);
        Destroy(gameObject);
    }
}
