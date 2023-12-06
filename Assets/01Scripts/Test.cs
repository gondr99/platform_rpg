using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    
    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            AnimatorEffect effect = PoolManager.Instance.Pop(PoolingType.HitImpactVFX) as AnimatorEffect;
            effect.PlayAnimation(transform.position, transform.rotation, Vector3.one);
        }
    }
}
