using System;
using System.Collections.Generic;


[Serializable]
public class GameData
{
    public int curreny;

    public SerializableDictionary<string, int> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentIDList;

    public SerializableDictionary<string, bool> checkpoints;
    public string lastVisitedCheckpointID; //마지막 방문한 체크포인트의 고유 아이디.
    
    public GameData()
    {
        curreny = 0;
        skillTree = new SerializableDictionary<string, int>();
        inventory = new SerializableDictionary<string, int>();
        equipmentIDList = new List<string>();
        checkpoints = new SerializableDictionary<string, bool>();
        lastVisitedCheckpointID = string.Empty;
    }
}