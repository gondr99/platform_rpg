using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFXPlayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _vfxPosition;
    [SerializeField] private Color _chillColor;
    [SerializeField] private Color _igniteColor;
    [SerializeField] private Color _shockColor;
    private Material _material;

    private readonly int _hashIsEffect = Shader.PropertyToID("_IsEffect");
    private readonly int _hashEffectColor = Shader.PropertyToID("_EffectColor");
    private readonly int _hashEffectIntensity = Shader.PropertyToID("_EffectIntensity");

    private ParticleEffect _ignite, _chill, _shock;
    
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
        //여긴 좀 개선할 수 있을거 같은데... 고민좀 해보자.
        if ((ailment & Ailment.Ignited) > 0 && _ignite == null)
        {
            _ignite = PoolManager.Instance.Pop(PoolingType.IgniteVFX) as ParticleEffect;
            PlayVFXParticle(_ignite);
        }
        else if((ailment & Ailment.Ignited) == 0 && _ignite != null)
        {
            _ignite.StopParticle();
            PoolManager.Instance.Push(_ignite);
            _ignite = null;
        }
        
        if ((ailment & Ailment.Shocked) > 0 && _shock == null)
        {
            _shock = PoolManager.Instance.Pop(PoolingType.ShockVFX) as ParticleEffect;
            PlayVFXParticle(_shock);
        }
        else if((ailment & Ailment.Shocked) == 0  && _shock != null)
        {
            _shock.StopParticle();
            PoolManager.Instance.Push(_shock);
            _shock = null;
        }
        
        if ((ailment & Ailment.Chilled) > 0 && _chill == null)
        {
            _chill = PoolManager.Instance.Pop(PoolingType.ChillVFX) as ParticleEffect;
            PlayVFXParticle(_chill);
        }
        else if((ailment & Ailment.Chilled) == 0 && _chill != null)
        {
            _chill.StopParticle();
            PoolManager.Instance.Push(_chill);
            _chill = null;
        }
      
    }

    private void PlayVFXParticle(ParticleEffect effect)
    {
        effect.transform.parent = transform;
        effect.transform.position = _vfxPosition.position;
        effect.PlayParticle();
    }
}
