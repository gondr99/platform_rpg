using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource[] _sfxArray;
    [SerializeField] private AudioSource[] _bgmArray;

    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < _sfxArray.Length)
        {
            _sfxArray[sfxIndex].Play();
        }
    }
}
