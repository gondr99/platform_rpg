using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BlinkFeedback : MonoBehaviour, Feedback
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _delaySecond;
    [SerializeField] private float _blinkValue;
    private readonly int _blinkShaderParam = Shader.PropertyToID("_BlinkValue");

    private Material _material;
    private bool _isFinished;
    private void Awake()
    {
        _material = _spriteRenderer.material;
    }

    public void CreateFeedback()
    {
        _material.SetFloat(_blinkShaderParam, _blinkValue);
        SetNormalAfterDelay(Mathf.FloorToInt(_delaySecond * 1000));
    }

    private async void SetNormalAfterDelay(int delayMS)
    {
        _isFinished = false;
        await Task.Delay(delayMS);

        if (_isFinished == false)
        {
            CompleteFeedback();
        }
        
    }

    public void CompleteFeedback()
    {
        _isFinished = true;
        _material.SetFloat(_blinkShaderParam, 0);
    }
}
