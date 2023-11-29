using UnityEngine;

public class InteractionFillUI : MonoBehaviour
{
    [SerializeField] private Transform _barTrm;
    [SerializeField] private Color _barColor;

    private void Awake()
    {
        _barTrm.Find("Fill").GetComponent<SpriteRenderer>().color = _barColor;
    }

    public void SetNormalizedGauge(float value)
    {
        _barTrm.localScale = new Vector3(value, 1, 1);
    }

    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }
}
