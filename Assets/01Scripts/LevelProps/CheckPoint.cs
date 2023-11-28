using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator _animator;

    private readonly int _hashActive = Animator.StringToHash("active");
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            //나중에 여기서 boolean값 주고 활성화 키 넣고 활성화키 누르면 세이브 포인트 활성화 되도록한다.
            _animator.SetBool(_hashActive, true);
        }
    }
}
