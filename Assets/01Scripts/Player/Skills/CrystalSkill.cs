using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private CrystalController _crystalPrefab;
    [SerializeField] private float _timeOut = 5f;
    private CrystalController _currentCrystal;
    
    public int damage = 5;
    public Vector2 knockPower;
    [Header("Moving Crystal")] 
    public bool canExplode;
    public float explosionRadius = 3f;

    [Header("Moving Crystal")] 
    public bool canMoveToEnemy;
    public float moveSpeed;

    public override void UseSkill()
    {
        base.UseSkill();

        if (_currentCrystal == null)
        {
            _currentCrystal = Instantiate(_crystalPrefab, _player.transform.position, Quaternion.identity);
            _currentCrystal.SetupCrystal(this,_timeOut);
        }
        else
        {
            //플레이어와 위치 바꾸기
            Vector2 playerPos = _player.transform.position;
            _player.transform.position = _currentCrystal.transform.position;
            _currentCrystal.transform.position = playerPos;
            
            _currentCrystal?.EndOfCrystal();
        }
    }

    public void UnlinkThisCrystal()
    {
        _currentCrystal = null;
    }
}
