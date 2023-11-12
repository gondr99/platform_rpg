using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Player _player;

    private void Awake()
    {
        _animator = transform.Find("Visual").GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 dir, float gravityScale, Player player)
    {
        _rigidbody.velocity = dir;
        _rigidbody.gravityScale = gravityScale;
        _player = player;
    }
}
