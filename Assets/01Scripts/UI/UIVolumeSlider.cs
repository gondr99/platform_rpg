using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parameter;
    
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _multiplier;

    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChaned);
    }

    public void HandleSliderValueChaned(float value)
    {
        _audioMixer.SetFloat(parameter, Mathf.Log10(value) * _multiplier);
    }
}
