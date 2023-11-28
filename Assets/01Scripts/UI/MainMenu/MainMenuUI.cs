using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneList _scene;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private FadeScreenUI _fadeScreen;

    private void Start()
    {
        if (SaveManager.Instance.HasSaveData() == false)
        {
            _continueButton.SetActive(false);
        }

        _fadeScreen.FadeIn(2f);
    }

    public void ContinueGame()
    {
        _fadeScreen.FadeOut(1f).OnComplete(() =>
        {
            SceneManager.LoadScene(_scene.ToString());
        });
    }

    public void NewGame()
    {
        SaveManager.Instance.DeleteSaveData();
        _fadeScreen.FadeOut(1f).OnComplete(() =>
        {
            SceneManager.LoadScene(_scene.ToString());
        });
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        //Application.Quit();
    }
}
