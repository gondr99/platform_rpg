using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private Transform contentTrm;

    private List<GameObject> _contentChilds = new List<GameObject>();
    private void Awake()
    {
        for (int i = 0; i < contentTrm.childCount; ++i)
        {
            _contentChilds.Add( contentTrm.GetChild(i).gameObject);
        }
        
    }

    public void ChangeTo(GameObject menu)
    {
        //모든 자식 끄고.
        foreach (GameObject panel in _contentChilds)
        {
            panel.SetActive(false);
        }

        if (menu != null)
        {
            menu.SetActive(true);
        }
    }
}
