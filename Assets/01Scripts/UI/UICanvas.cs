using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private RectTransform _menuTrm;
    [SerializeField] private Transform contentTrm;
    [SerializeField] private InputReader _inputReader;

    private List<GameObject> _contentChilds = new List<GameObject>();

    private bool _isOpenMenu = false;
    
    private void Awake()
    {
        for (int i = 0; i < contentTrm.childCount; ++i)
        {
            _contentChilds.Add( contentTrm.GetChild(i).gameObject);
        }

        CloseWindow();
    }

    private void OnEnable()
    {
        _inputReader.OpenMenuEvent += HandleOpenWindow;
    }

    private void OnDisable()
    {
        _inputReader.OpenMenuEvent -= HandleOpenWindow;
    }

    private void HandleOpenWindow()
    {
        if(_isOpenMenu) CloseWindow();
        else OpenWindow();
    }
    private void OpenWindow()
    {
        Time.timeScale = 0;
        _inputReader.SetPlayerInputEnable(false);
        
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(_menuTrm.DOAnchorPos(new Vector2(50, 50), 0.5f));
        seq.Join(_menuTrm.DOScaleY(1, 0.5f));
        seq.AppendCallback(() => _isOpenMenu = true);
    }

    private void CloseWindow()
    {
        Time.timeScale = 1;
        _inputReader.SetPlayerInputEnable(true);
        
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(_menuTrm.DOAnchorPos(new Vector2(Screen.width, 50), 0.5f) );
        seq.Join(_menuTrm.DOScaleY(0.5f, 0.5f));
        seq.AppendCallback(() => _isOpenMenu = false);
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
