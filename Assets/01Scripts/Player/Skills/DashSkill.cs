using UnityEngine;

public class DashSkill : Skill
{
    public override void UseSkill()
    {
        base.UseSkill();
        
        Debug.Log("뒤쪽에 클론 생성하기.");
    }
}
