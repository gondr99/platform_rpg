using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private float _sfxMinimumDistance = 10f;
    [SerializeField] private AudioSource[] _sfxArray;
    [SerializeField] private AudioSource[] _bgmArray;

    public bool playBGM;
    private int _currentBGMIndex = 0;
    private bool _canPlaySFX; //최초 로딩시에는 재생하지 않도록 하는 변수

    private void Awake()
    {
        //시작하고 1초후에 SFX 재생 허락함. 이건 쓰레드써도 돼
        AllowSFX(1000); 
    }

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

    //SFX를 재생하도록 하는 함수.
    private async void AllowSFX(int milliSec)
    {
        await Task.Delay(milliSec);
        _canPlaySFX = true;
    } 
    
        
    
    #region 재생 컨트롤
    
    public void PlaySFX(int sfxIndex, Transform sourceTrm, bool withRandomPitch = false)
    {
        if (_canPlaySFX == false) return; //준비가 되지 않았다면 패스
        
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _sfxMinimumDistance);
        Gizmos.color = Color.white;
    }

    #endif
}
