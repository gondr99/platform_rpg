using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFXPlayer : EntityFXPlayer
{
    [SerializeField] protected ParticleSystem _dustEffect;

    public void PlayDustEffect()
    {
        _dustEffect.Play();
    }
}
