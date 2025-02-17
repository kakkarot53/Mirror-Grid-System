using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is an instance, use this to call whenever you need to load and/or save data
//if only one function is necessary, leave the other blank
public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(GameData data);
}
