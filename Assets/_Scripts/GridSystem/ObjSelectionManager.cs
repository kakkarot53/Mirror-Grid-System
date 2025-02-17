using Hypertonic.GridPlacement.Example.BasicDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ObjSelectionManager : MonoBehaviour
{
    [SerializeField]
    private Button_GridObjectSelectionOption btnPrefab;
    [SerializeField]
    private Transform content;

    private List<AsyncOperationHandle<PrefabDataObj>> loadedHandles = new List<AsyncOperationHandle<PrefabDataObj>>();

    private void Start()
    {
        LoadAllPrefabs();
    }

    private void LoadAllPrefabs()
    {
        // Load all Addressables labeled "PrefabToSpawn"
        Addressables.LoadResourceLocationsAsync("DataToSpawn").Completed += OnResourceLocationsLoaded;
    }

    private void OnResourceLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Folder accessed");

            foreach (var location in handle.Result)
            {
                Debug.Log($"Loaded resource location: {location.PrimaryKey}");

                StartCoroutine(LoadAndCreateButton(location));
                //LoadBtn(location);
            }
        }
        else
        {
            Debug.LogError("Failed to load resource locations for PrefabToSpawn");
        }
    }

    //private async void LoadBtn(IResourceLocation location)
    //{
    //    AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(location);
    //    loadedHandles.Add(loadHandle);

    //    if (loadHandle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        GameObject obj = loadHandle.Result;

    //        // Instantiate the button and set it up with the loaded prefab
    //        Button_GridObjectSelectionOption button = Instantiate(btnPrefab, content);
    //        button.gameObject.name = "Button_" + obj.name;
    //        button.BtnSetup(obj);
    //    }
    //    else
    //    {
    //        Debug.LogError($"Failed to load prefab at address: {location.PrimaryKey}");
    //    }
    //}

    private IEnumerator LoadAndCreateButton(IResourceLocation location)
    {
        // Load the prefab asynchronously by address
        AsyncOperationHandle<PrefabDataObj> loadHandle = Addressables.LoadAssetAsync<PrefabDataObj>(location);
        loadedHandles.Add(loadHandle); // Store handle to release later
        yield return loadHandle;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = loadHandle.Result.prefabToSpawn;

            // Instantiate the button and set it up with the loaded prefab
            Button_GridObjectSelectionOption button = Instantiate(btnPrefab, content);
            button.gameObject.name = "Button_" + obj.name;
            button.BtnSetup(loadHandle.Result);
        }
        else
        {
            Debug.LogError($"Failed to load prefab at address: {location.PrimaryKey}");
        }
    }

    private void OnDestroy()
    {
        // Release all loaded addressable assets to free memory
        foreach (var handle in loadedHandles)
        {
            Addressables.Release(handle);
        }
    }

    //OLD START FUN
    //private void Start()
    //{
    //    // Load all objects in the Resources folder
    //    GameObject[] objData = Resources.LoadAll<GameObject>("PrefabToSpawn");

    //    foreach (GameObject obj in objData)
    //    {
    //        // Instantiate the button prefab
    //        Button_GridObjectSelectionOption button = Instantiate(btnPrefab, content);

    //        button.gameObject.name = "Button_" + obj.name;
    //        button.BtnSetup(obj);
    //    }
    //}
}
