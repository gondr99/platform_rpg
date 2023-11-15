using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone_info")]
    [SerializeField] private CloneSkillController _clonePrefab;
    [SerializeField] private float _cloneDuration;
    [SerializeField] private bool _canAttack;

    public void CreateClone(Transform originTrm, Vector3 offset = new Vector3())
    {
        CloneSkillController newClone = Instantiate(_clonePrefab);
        newClone.SetupClone(originTrm, offset, _cloneDuration, _canAttack);
    }
}
