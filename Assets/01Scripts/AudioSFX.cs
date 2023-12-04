using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioSFX : MonoBehaviour
{
    private AudioSource _source;
    private float _basePitch;
    private float _baseVolume;
    
    [Range(0, 1f)] [SerializeField]  private float _pitchRandomness;

    public bool IsPlaying => _source.isPlaying;

    private bool _isTweening;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _basePitch = _source.pitch;
        _baseVolume = _source.volume;
    }

    public void PlaySource(bool withRandomPitch)
    {
        if (_source.isPlaying && !_isTweening) return;
        
        _source.DOKill();
        _isTweening = false;
        _source.volume = _baseVolume;
        if (withRandomPitch)
            _source.pitch = _basePitch + Random.Range(-_pitchRandomness, _pitchRandomness);
        _source.Play();
    }

    public void StopSource(bool isFade)
    {
        if (isFade)
        {
            _isTweening = true;
            _source.DOFade(0, 1.5f).OnComplete(() =>
            {
                _isTweening = false;
                _source.Stop();
            });
        }
        else
        {
            _source.Stop();
        }
    }
}
