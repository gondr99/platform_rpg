using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private float _maxSize;
    [SerializeField] private float _growSpeed;
    [SerializeField] private bool _canGrow;

    public List<Enemy> targets;
    private void Update()
    {
        if (_canGrow)
        {
            transform.localScale =
                Vector2.Lerp(transform.localScale, Vector2.one * _maxSize, _growSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            targets.Add(enemy);
        }
    }
}
