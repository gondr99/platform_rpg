using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>, ISaveManager
{
    [SerializeField] private Player _player;
    public Transform PlayerTrm => _player.transform;
    public Player Player => _player;

    private Camera _mainCam;

    public Camera MainCam
    {
        get {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            return _mainCam;
        }
    }

    [SerializeField] private CheckPoint[] _checkPoints;
    private string _lastVisitedCheckPointId = string.Empty;
    private void Awake()
    {
        _checkPoints = FindObjectsOfType<CheckPoint>();
        CheckPoint.CheckPointActiveEvent += HandleCheckPointActiveEvent;
    }
    
    protected virtual void OnDestroy()
    {
        CheckPoint.CheckPointActiveEvent -= HandleCheckPointActiveEvent;
    }

    private void HandleCheckPointActiveEvent(string checkpointID)
    {
        _lastVisitedCheckPointId = checkpointID;
    }



    public void RestartGame()
    {
        SceneManager.LoadScene(SceneList.GameScene.ToString());
    }

    public void LoadData(GameData data)
    {

        foreach (var point in _checkPoints)
        {
            if (data.checkpoints.TryGetValue(point.checkpointID, out bool value))
            {
                if (value)
                {
                    point.ActiveCheckPoint();
                }
            }
        }
        _lastVisitedCheckPointId = data.lastVisitedCheckpointID; //마지막 방문 체크포인트 저장.

        CheckPoint last = GetLastCheckPoint();
        if(last != null)
            _player.transform.position = last.transform.position;
    }

    public void SaveData(ref GameData data)
    {
        data.checkpoints.Clear();
        foreach (var point in _checkPoints)
        {
            data.checkpoints.Add(point.checkpointID, point.isActivated);
        }

        data.lastVisitedCheckpointID = _lastVisitedCheckPointId;
    }


    public CheckPoint GetLastCheckPoint()
    {
        if (_lastVisitedCheckPointId == string.Empty) return null;

        foreach (var point in _checkPoints)
        {
            if (point.checkpointID == _lastVisitedCheckPointId)
                return point;
        }

        return null;
    }
}
