using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFXPlayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Color _chillColor;
    [SerializeField] private Color _igniteColor;
    [SerializeField] private Color _shockColor;
    private Material _material;

    private readonly int _hashIsEffect = Shader.PropertyToID("_IsEffect");
    private readonly int _hashEffectColor = Shader.PropertyToID("_EffectColor");
    private readonly int _hashEffectIntensity = Shader.PropertyToID("_EffectIntensity");
    private void Awake()
    {
        _material = _spriteRenderer.material;
    }

    //디버프 상태를 받아서 표기해주는 함수
    public void HandleAilmentState(Ailment ailment)
    {
        
        _material.SetInt(_hashIsEffect, ailment > 0 ? 1 : 0);
        
        //우선순위로 비쥬얼 적용. 점화->감전->동결
        if ((ailment & Ailment.Ignited) > 0)
        {
            _material.SetColor(_hashEffectColor, _igniteColor);
        } 
        else if ((ailment & Ailment.Shocked) > 0)
        {
            _material.SetColor(_hashEffectColor, _shockColor);
        }
        else if ((ailment & Ailment.Chilled) > 0)
        {
            _material.SetColor(_hashEffectColor, _chillColor);
        }
        
        VisualizeIcon(ailment);
    }

    private void VisualizeIcon(Ailment ailment)
    {
        //나중에 여기서 아이콘 표기 변경
    }
}
