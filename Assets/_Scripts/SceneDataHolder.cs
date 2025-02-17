using UnityEngine;
using Mirror;
public class SceneDataHolder : NetworkBehaviour
{
    public static SceneDataHolder Instance { get; private set; }

    [SyncVar(hook = nameof(OnSaveIDChanged))]
    private string saveID = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSaveID(string _saveID)
    {
        if (isServer)
        {
            saveID = _saveID;
            Debug.Log($"Host set Save ID: {saveID}");
        }
    }

    private void OnSaveIDChanged(string oldID, string newID)
    {
        Debug.Log($"Save ID changed: {newID}");
        if (isClient && !isServer)
        {
            LocalDataHolder.Instance.SetSceneId(newID.ToString());
            DataPersistenceManager.instance.ChangeSelectedProfileId(newID);

            //SceneLoader.Instance.LoadScene(newID); // Load scene based on save ID
        }
    }
}
