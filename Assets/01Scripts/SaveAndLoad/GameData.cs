using System;
using System.Collections.Generic;


[Serializable]
public class GameData
{
    public int curreny;

    public SerializableDictionary<string, int> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentIDList;
    
    public GameData()
    {
        curreny = 0;
        skillTree = new SerializableDictionary<string, int>();
        inventory = new SerializableDictionary<string, int>();
        equipmentIDList = new List<string>();
    }
}
