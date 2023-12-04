using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int _areaSoundIndex;
    [SerializeField] private bool _isFade;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            AudioManager.Instance.PlaySFX(_areaSoundIndex, null);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            AudioManager.Instance.StopSFX(_areaSoundIndex, _isFade);
        }
    }

    
}
