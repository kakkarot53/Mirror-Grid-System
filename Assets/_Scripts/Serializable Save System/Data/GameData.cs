using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public string name;
    //save tile data stuff
    public List<GridObjectSaveDataNew> gridSaveData = new List<GridObjectSaveDataNew>();

    public GameData()
    {

    }

    public GameData SetName(string _n)
    {
        name = _n;
        return this;
    }
}
