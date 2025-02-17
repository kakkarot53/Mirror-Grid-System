using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections;
using System.Threading.Tasks;

public class SaveLoadManager : MonoBehaviour, IDataPersistence
{
    public static SaveLoadManager instance;

    [SerializeField]
    private Button saveBtn;
    [SerializeField]
    private Button loadBtn;

    public List<GameObject> objData = new List<GameObject>();

    private Dictionary<string, PrefabDataObj> loadedData = new Dictionary<string, PrefabDataObj>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        saveBtn.onClick.RemoveAllListeners();
        loadBtn.onClick.RemoveAllListeners();

        saveBtn.onClick.AddListener(DataPersistenceManager.instance.SaveGame);
        loadBtn.onClick.AddListener(DataPersistenceManager.instance.LoadGame);
    }
    private async Task LoadAllAdressableDatas()
    {
        loadedData.Clear();

        // Load all Addressables labeled "PrefabToSpawn", wait until task is finished before continuing
        var locationDatas = Addressables.LoadResourceLocationsAsync("DataToSpawn");
        await locationDatas.Task;

        //if folder is accessed
        if(locationDatas.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Folder accessed");
            //for all data that is found in datatospawn addressable
            foreach (var location in locationDatas.Result)
            {
                //Debug.Log($"Loaded resource location: {location.PrimaryKey}");

                var loadHandle = Addressables.LoadAssetAsync<PrefabDataObj>(location);
                await loadHandle.Task;

                //if it cant find it return
                if (loadHandle.Status != AsyncOperationStatus.Succeeded)
                    return;

                //take data only if there is nobody with the same name and add it to the dictionary
                //uses a dictionary to save 2 vars at once, one is a key and one is a data
                PrefabDataObj prefabData = loadHandle.Result;

                if (!loadedData.ContainsKey(prefabData.name))
                {
                    loadedData.Add(prefabData.name, prefabData);
                    Debug.Log($"Added {prefabData.name}, {prefabData.prefabToSpawn.name}");
                }
            }
        }
    }

    public async void LoadData(GameData data)
    {
        if (objData.Count > 0)
            objData.Clear();

        await LoadAllAdressableDatas(); //load all data in addressables

        // Group saved data by unique GridKey
        Dictionary<string, List<GridObjectPositionData>> gridDataByKeys = new Dictionary<string, List<GridObjectPositionData>>();

        foreach (GridObjectSaveDataNew savedData in data.gridSaveData)
        {
            //basically checks is savedData.prefabName exists in the loaded data or not
            if (loadedData.TryGetValue(savedData.PrefabName, out PrefabDataObj prefabData))
            {
                GameObject gridObject = Instantiate(prefabData.prefabToSpawn);
                gridObject.transform.rotation = savedData.ObjectRotation;

                gridObject.name = prefabData.name;

                // Add required components if missing
                if (!gridObject.TryGetComponent(out ExampleGridObject exampleGridObjectComponent))
                {
                    gridObject.AddComponent<ExampleGridObject>();
                }

                if (!gridObject.TryGetComponent(out GridObjectInfo objInfo))
                {
                    objInfo = gridObject.AddComponent<GridObjectInfo>();
                    objInfo.Setup(savedData.GridName, savedData.GridCellIndex, savedData.ObjectAlignment);
                }

                objData.Add(gridObject);

                // Group GridObjectPositionData by GridKey
                if (!gridDataByKeys.ContainsKey(savedData.GridName))
                {
                    gridDataByKeys[savedData.GridName] = new List<GridObjectPositionData>();
                }

                GridObjectPositionData gridObjectPositionData = new GridObjectPositionData(gridObject, savedData.GridCellIndex, savedData.ObjectAlignment);
                gridDataByKeys[savedData.GridName].Add(gridObjectPositionData);
            }
        }

        // Populate each grid with its associated data
        foreach (var gridEntry in gridDataByKeys)
        {
            GridData gridData = new GridData(gridEntry.Value);
            _ = GridManagerAccessor.GetGridManagerByKey(gridEntry.Key).PopulateWithGridDataAsync(gridData, true);
        }
    }

    public void SaveData(GameData data)
    {
        data.gridSaveData.Clear();

        foreach (GameObject g in objData)
        {
            if (g != null)
            {
                GridObjectInfo objInfo = g.GetComponent<GridObjectInfo>();

                if (objInfo == null)
                    return;

                GridObjectSaveDataNew _data = new GridObjectSaveDataNew(g.name,
                    objInfo.GridKey,
                    g.transform.position,
                    objInfo.GridCellIndex,
                    objInfo.ObjectAlignment,
                    g.transform.localRotation);

                //saves data filled above
                data.gridSaveData.Add(_data);
            }
        }
    }
}

[System.Serializable]
public class GridObjectSaveDataNew
{
    public string PrefabName;
    public string GridName;
    public Vector3 ObjectPos; // obsolete
    public Vector2Int GridCellIndex;
    public ObjectAlignment ObjectAlignment;
    public Quaternion ObjectRotation;

    public GridObjectSaveDataNew(string prefabKey,string gridKey, Vector3 objectPos, Vector2Int gridCellIndex, ObjectAlignment objectAlignment, Quaternion objectRotation)
    {
        PrefabName = prefabKey; // search item in /resource
        GridName = gridKey;     //search grid settings
        ObjectPos = objectPos;  
        GridCellIndex = gridCellIndex; //populate data in grid
        ObjectAlignment = objectAlignment; 
        ObjectRotation = objectRotation;
    }
}
