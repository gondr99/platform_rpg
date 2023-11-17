using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform _fillTrm;

    
    public void SetNormalizedHealth(float value)
    {
        _fillTrm.DOKill();
        _fillTrm.DOScaleX(value, 0.2f);
    }

    public void FlipUI()
    {
        transform.Rotate(0, 180, 0); //180도 회전.
    }
}
