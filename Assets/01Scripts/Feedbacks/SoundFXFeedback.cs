using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXFeedback : MonoBehaviour, Feedback
{ 
    [SerializeField] private int _playSoundIndex;
    [SerializeField] private bool _isRandomPitch;
    public void CreateFeedback()
    {
        AudioManager.Instance.PlaySFX(_playSoundIndex, null,_isRandomPitch);
    }

    public void CompleteFeedback()
    {
        
    }
}
