using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//3초에 한번씩 3타마다 썬더 스트라이크가 사거리내 랜덤 적에게 확률적으로 발동
public class ThunderStrikeSkill : Skill
{
    [Header("Skill info")] 
    [SerializeField] private ThunderStrikeController _skillPrefab;
    public bool isEnabled;  //활성화 여부.
    public float effectRadius = 15f;
    public int activePercent = 30; //30퍼 확률로 발동.

    public int amountOfThunder = 1; //떨어지는 번개 수 
    [Header("Ailment")] 
    public bool isShockable; //감점가능

    private List<Enemy> _targetList = new List<Enemy>();

    public override void UseSkill()
    {
        base.UseSkill();
        if(!isEnabled) return; //비활성화시 작동안함.

        if (Random.Range(0, 100) > activePercent)
            return;
        
        //확률 통과했다면 시작.
        FillTargetList();
        DamageToTargets();
    }

    //확률이니 이펙트 상관없이 발동하는것
    public override void UseSkillWithoutCooltimeAndEffect()
    {
        FillTargetList();
        DamageToTargets();
    }

    private async void DamageToTargets()
    {
        Vector3 offset = new Vector3(0, 3.5f); //머리위에서 부터
        foreach (Enemy enemy in _targetList)
        {
            ThunderStrikeController thunderInstance = Instantiate(_skillPrefab, enemy.transform.position + offset, Quaternion.identity);
            thunderInstance.Setup(this, enemy);
            await Task.Delay(300);
        }
    }

    private void FillTargetList()
    {
        _targetList.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.transform.position, effectRadius, whatIsEnemy);

        if (colliders.Length == 0) return; //아무것도 없다면 할게 없다.
        
        for (int i = 0; i < amountOfThunder; ++i)
        {
            Enemy enemy = colliders[Random.Range(0, colliders.Length)].GetComponent<Enemy>();
            _targetList.Add( enemy ); 
        }
        
    }
}
