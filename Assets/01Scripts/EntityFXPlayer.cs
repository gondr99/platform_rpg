using System;
using Cinemachine;
using UnityEngine;

public class EntityFXPlayer : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Transform _vfxPosition;
    [SerializeField] protected Color _chillColor;
    [SerializeField] protected Color _igniteColor;
    [SerializeField] protected Color _shockColor;
    protected Material _material;

    protected readonly int _hashIsEffect = Shader.PropertyToID("_IsEffect");
    protected readonly int _hashEffectColor = Shader.PropertyToID("_EffectColor");
    protected readonly int _hashEffectIntensity = Shader.PropertyToID("_EffectIntensity");

    protected ParticleEffect _ignite, _chill, _shock;

    protected CinemachineImpulseSource _impulseSource;
    

    [Header("AfterImage")] 
    [SerializeField] protected float _afterImageInterval = 0.03f;
    [SerializeField] protected float _afterImageLivetime = 0.4f; 
    [SerializeField] protected bool _afterImageMode;
    protected float _currentTimer = 0f;

    protected Player _player;
    protected virtual void Awake()
    {
        _material = _spriteRenderer.material;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(Vector2 direction)
    {
        _impulseSource.GenerateImpulseWithVelocity(direction);
    }
    
    protected virtual void Start()
    {
        _player = GameManager.Instance.Player;
    }

    #region after image generator
    public void SetAfterImageMode(bool value)
    {
        _afterImageMode = value;
    }

    protected virtual void Update()
    {
        if (_afterImageMode)
        {
            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0)
            {
                AfterImage afterImage = PoolManager.Instance.Pop(PoolingType.AfterImage) as AfterImage;
                if (afterImage != null)
                {
                    Vector3 position = _spriteRenderer.transform.position;
                    Sprite sprite = _spriteRenderer.sprite;
                    bool isFlip = _player.FacingDirection == -1;
                    afterImage.StartFade(position, sprite, _afterImageLivetime, isFlip);
                    _currentTimer = _afterImageInterval;
                }
            }
        }
    }
    

    #endregion
    
    #region ailment effect
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

    protected virtual void VisualizeIcon(Ailment ailment)
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
            PoolManager.Instance.Push(_ignite, true);
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
            PoolManager.Instance.Push(_shock, true);
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
            PoolManager.Instance.Push(_chill, true);
            _chill = null;
        }
      
    }

    protected void PlayVFXParticle(ParticleEffect effect)
    {
        effect.transform.parent = transform;
        effect.transform.position = _vfxPosition.position;
        effect.PlayParticle();
    }

    #endregion
}
