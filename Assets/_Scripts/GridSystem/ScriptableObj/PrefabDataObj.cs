using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDataObj", menuName = "Scriptable Objects/PrefabDataObj")]
public class PrefabDataObj : ScriptableObject
{
    //public int ID;
    public GameObject prefabToSpawn;
    public Sprite spriteToSpawn;
}
