using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private float _sfxMinimumDistance = 10f;
    [SerializeField] private AudioSource[] _sfxArray;
    [SerializeField] private AudioSource[] _bgmArray;

    public bool playBGM;
    private int _currentBGMIndex = 0;
    
    private void Update()
    {
        if (!playBGM)
        {
            StopAllBGM();
        }
        else if(_bgmArray[_currentBGMIndex].isPlaying == false)
        {
            _bgmArray[_currentBGMIndex].Play();
        }
    }

    #region 재생 컨트롤
    
    public void PlaySFX(int sfxIndex, Transform sourceTrm, bool withRandomPitch = false)
    {
        Transform playerTrm = GameManager.Instance.PlayerTrm;
        if (sourceTrm != null && Vector2.Distance(sourceTrm.position, playerTrm.position) > _sfxMinimumDistance)
        {
            return;
        }
        
        if (sfxIndex < _sfxArray.Length)
        {
            if (_sfxArray[sfxIndex].isPlaying) return; //재생중이면 리턴.
            _sfxArray[sfxIndex].pitch = withRandomPitch ? Random.Range(0.85f, 1.15f) : 1f;
            _sfxArray[sfxIndex].Play();
        }
    }

    public void StopSFX(int sfxIndex)
    {
        _sfxArray[sfxIndex].Stop();
    }

    public void PlayRandomBGM()
    {
        int idx = Random.Range(0, _bgmArray.Length);
        PlayBGM(idx);
    }
    
    public void PlayBGM(int bgmIndex)
    {
        _bgmArray[_currentBGMIndex].Stop();
        _bgmArray[bgmIndex].Play();
        _currentBGMIndex = bgmIndex;
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < _bgmArray.Length; ++i)
        {
            _bgmArray[i].Stop();    
        }
    }

    #endregion
    
    
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _sfxMinimumDistance);
        Gizmos.color = Color.white;
    }
#endif
}
