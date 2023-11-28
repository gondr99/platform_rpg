using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoSingleton<SaveManager>
{
    [SerializeField] private string fileName;
    
    private GameData _gameData;
    private List<ISaveManager> _saveManagerList;
    private FileDataHandler _fileDataHandler;
    private void Start()
    {
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, true);
        _saveManagerList = FindAllSaveManagers();
        
        LoadGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void LoadGame()
    {
        _gameData = _fileDataHandler.Load();
        if (_gameData == null)
        {
            Debug.Log("No save data found");
            NewGame();
        }
        
        foreach (ISaveManager manager in _saveManagerList)
        {
            manager.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager manager in _saveManagerList)
        {
            manager.SaveData(ref _gameData);
        }
        
        _fileDataHandler.Save(_gameData);
    }


    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        //인터페이스는 모노 타입이 아니라서 가져올 수 없어.
        return FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>().ToList();
    }

    [ContextMenu("Delete save file")]
    private void DeleteSaveData()
    {
        //컨텍스트에서 실행시 객체가 없어서 이거 만들어서 해야함.
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, true);
        _fileDataHandler.DeleteSaveData();
    }
}
