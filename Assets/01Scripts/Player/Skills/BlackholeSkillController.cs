using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private float _maxSize;
    [SerializeField] private float _growSpeed;
    [SerializeField] private bool _canGrow;

    [SerializeField] private float _maxRiffleCount = 2;
    [SerializeField] private float _maxRiffleSpeed = 1f;
    
    private readonly int _HashRiffleSpeed = Shader.PropertyToID("_RiffleSpeed");
    private readonly int _HashRiffleCount = Shader.PropertyToID("_RiffleCount");

    public List<Enemy> targets;
    private Material _riffleMat;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _riffleMat = _spriteRenderer.material;
        _riffleMat.SetFloat(_HashRiffleSpeed, 0);
        _riffleMat.SetFloat(_HashRiffleCount, 0);
    }

    private void Update()
    {
        if (_canGrow)
        {
            transform.localScale =
                Vector3.Lerp(transform.localScale, Vector3.one * _maxSize, _growSpeed * Time.deltaTime);

            float currentRiffleSpeed = _riffleMat.GetFloat(_HashRiffleSpeed);
            currentRiffleSpeed = Mathf.Lerp(currentRiffleSpeed, _maxRiffleSpeed, _growSpeed * Time.deltaTime);
            _riffleMat.SetFloat(_HashRiffleSpeed, currentRiffleSpeed);
            
            
            float currentRiffleCount = _riffleMat.GetFloat(_HashRiffleCount);
            currentRiffleCount = Mathf.Lerp(currentRiffleCount, _maxRiffleSpeed, _growSpeed * Time.deltaTime);
            _riffleMat.SetFloat(_HashRiffleCount, currentRiffleCount);
            
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
