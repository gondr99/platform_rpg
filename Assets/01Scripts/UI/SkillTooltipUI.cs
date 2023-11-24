using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private TextMeshProUGUI _skillDescription;

    public void ShowTooltip(string skillName, string skillDesc)
    {
        _skillName.text = skillName;
        _skillDescription.text = skillDesc;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
