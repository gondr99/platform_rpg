using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GameData
{
    public int curreny;

    public SerializableDictionary<string, int> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentIDList;

    public SerializableDictionary<string, bool> checkpoints;
    public string lastVisitedCheckpointID; //마지막 방문한 체크포인트의 고유 아이디.

    public int exp;
    public int level = 1;
    public int statPoint;
    public int skillPoint;

    public SerializableDictionary<string, float> volumeSettings;

    public GameData()
    {
        curreny = 0;
        level = 1;
        skillTree = new SerializableDictionary<string, int>();
        inventory = new SerializableDictionary<string, int>();
        equipmentIDList = new List<string>();
        checkpoints = new SerializableDictionary<string, bool>();
        lastVisitedCheckpointID = string.Empty;
        volumeSettings = new SerializableDictionary<string, float>();
    }
}
