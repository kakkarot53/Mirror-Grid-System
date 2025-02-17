using UnityEngine;

public class LocalDataHolder : MonoBehaviour
{
    public static LocalDataHolder Instance;
    public string nickName { private set; get; }
    public string id { private set; get; }

    private void Awake()
    {
        // Ensure only one instance of this object exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetNickname(string name)
    {
        nickName = name;
    }
    public void SetSceneId(string _id)
    {
        id = _id;
    }
}
