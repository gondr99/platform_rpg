using System;
using UnityEngine;

public class IceAndFireController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    private float _lifeTime;
    private float _currentTime;
    // 2초후 제거, 닿으면 폭발하며 빙결및 점화 상태이상
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FireToFront(Vector2 velocity, float lifetime = 2f)
    {
        _currentTime = 0;
        _lifeTime = lifetime;
        _rigidbody.velocity = velocity;
        if (velocity.x < 0)
        {
            transform.Rotate(0, 180f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Player player = GameManager.Instance.Player;
            int magicDamage = player.Stat.fireDamage.GetValue() + player.Stat.iceDamage.GetValue();
            enemy.HealthCompo.ApplyMagicDamage(magicDamage, _rigidbody.velocity.normalized, new Vector2(3, 5), player);
            
            //동상 걸고, 화상걸고.
            enemy.HealthCompo.SetAilment(Ailment.Chilled, 2f, player.Stat.GetDotDamage(Ailment.Chilled));
            enemy.HealthCompo.SetAilment(Ailment.Ignited, 3f, player.Stat.GetDotDamage(Ailment.Ignited));
            Explosion();
        }
    }

    private void FixedUpdate()
    {
        _currentTime += Time.fixedDeltaTime;
        if (_currentTime >= _lifeTime)
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        //나중에 여기 이펙트 추가.
        Destroy(gameObject);
    }
}
