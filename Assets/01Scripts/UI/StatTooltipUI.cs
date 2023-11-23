using TMPro;
using UnityEngine;

public class StatTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void ShowStatTooltip(string text)
    {
        _descriptionText.text = text;
        
        gameObject.SetActive(true);
    }

    public void HideStatTooltip()
    {
        _descriptionText.text = string.Empty;
        gameObject.SetActive(false);
    }
}
