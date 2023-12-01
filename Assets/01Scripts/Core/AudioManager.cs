using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoSingleton<AudioManager>
{
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
    
    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < _sfxArray.Length)
        {
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
}
