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

    private CanvasGroup _menuCanvasGroup;
    private bool _isOpenMenu = false;
    
    private void Awake()
    {
        for (int i = 0; i < contentTrm.childCount; ++i)
        {
            _contentChilds.Add( contentTrm.GetChild(i).gameObject);
        }
        
        _menuTrm.gameObject.SetActive(true);
        _menuCanvasGroup = _menuTrm.GetComponent<CanvasGroup>();
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
        float tweenTime = 0.3f;
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(_menuTrm.DOAnchorPos(new Vector2(50, 50), tweenTime));
        seq.Join(_menuTrm.DOScaleY(1, tweenTime));
        seq.Join(_menuCanvasGroup.DOFade(1, tweenTime));
        seq.AppendCallback(() => _isOpenMenu = true);
    }

    private void CloseWindow()
    {
        Time.timeScale = 1;
        _inputReader.SetPlayerInputEnable(true);
        float tweenTime = 0.3f;
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(_menuTrm.DOAnchorPos(new Vector2(Screen.width, 50), tweenTime) );
        seq.Join(_menuTrm.DOScaleY(0.5f, tweenTime));
        seq.Join(_menuCanvasGroup.DOFade(0, tweenTime));
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
