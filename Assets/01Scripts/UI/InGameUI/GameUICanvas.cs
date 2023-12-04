using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUICanvas : MonoBehaviour, ISaveManager
{
    [SerializeField] private RectTransform _menuTrm;
    [SerializeField] private Transform contentTrm;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private FadeScreenUI _fadeScreenUI;
    [SerializeField] private TextMeshProUGUI _youDieText;
    
    private List<GameObject> _contentChilds = new List<GameObject>();

    private CanvasGroup _menuCanvasGroup;
    private bool _isOpenMenu = false;

    [SerializeField] private UIVolumeSlider[] _volumeSettings;
    
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

    private void Start()
    {
        _fadeScreenUI.ResetScreen(true); //검은색으로 칠하고
        _fadeScreenUI.FadeIn(0.5f);
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
        AudioManager.Instance.PlaySFX(7, null);
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
        AudioManager.Instance.PlaySFX(7, null);
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
            AudioManager.Instance.PlaySFX(7, null);
        }
    }

    public void HandleRestartGame()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void SetDeadFadeOut()
    {
        _fadeScreenUI.FadeOut(3f).OnComplete( () =>
        {
            StartCoroutine(TypeWrite());
        });
    }

    IEnumerator TypeWrite()
    {
        _youDieText.gameObject.SetActive(true);
        _youDieText.ForceMeshUpdate();
        int totalCount = _youDieText.textInfo.characterCount;
        int count = 0;
        while (true)
        {
            _youDieText.maxVisibleCharacters = count;
            yield return new WaitForSeconds(0.1f);
            ++count;
            if (count > totalCount)
            {
                break;
            }
        }
    }

    public void LoadData(GameData data)
    {
        for (int i = 0; i < _volumeSettings.Length; ++i)
        {
            UIVolumeSlider item = _volumeSettings[i];
            if (data.volumeSettings.TryGetValue(item.parameter, out float value))
            {
                item.slider.value = value;
                item.HandleSliderValueChaned(value);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.volumeSettings.Clear();
        for (int i = 0; i < _volumeSettings.Length; ++i)
        {
            UIVolumeSlider item = _volumeSettings[i];
            data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }
}
