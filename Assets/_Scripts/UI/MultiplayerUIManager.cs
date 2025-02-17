using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class MultiplayerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputName;

    [Header("Join Room Panel")]
    [SerializeField] private TMP_InputField inputIP;
    [SerializeField] private TMP_InputField inputPort;

    [SerializeField] private Button btnHost;
    [SerializeField] private Button btnClient;

    [SerializeField]
    private SceneSlot[] sceneSlots;

    private string selectedSaveID = null;

    private void Start()
    {
        // Init the input field with Network Manager's network address.
        inputIP.text = NetworkManager.singleton.networkAddress;
        GetPort();

        RegisterListeners();
    }

    private void OnEnable()
    {
        LoadData();
    }

    private void LoadData()
    {
        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // loop through each save slot in the UI and set the content appropriately
        foreach (SceneSlot saveSlot in sceneSlots)
        {
            GameData _profileData;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out _profileData);
            saveSlot.SetData(_profileData);
            bool isAvailable = _profileData != null;

            saveSlot.SetInteractable(isAvailable);

            if (isAvailable)
            {
                saveSlot.selectBtn.onClick.RemoveAllListeners();
                saveSlot.selectBtn.onClick.AddListener(() => OnSelectSaveSlot(saveSlot.GetProfileId()));
            }
        }
    }
    private void OnSelectSaveSlot(string profileId)
    {
        selectedSaveID = profileId;
        LocalDataHolder.Instance.SetSceneId(profileId);
    }
    private void RegisterListeners()
    {
        //for name panel
        inputName.onEndEdit.AddListener(delegate { CheckName(); });

        //for networking panel
        btnHost.onClick.AddListener(() =>
        {
            CheckName();
            if (string.IsNullOrEmpty(selectedSaveID))
            {
                Debug.LogWarning("No save slot selected!");
                return;
            }
            OnClickStartHostButton();
        });
        btnClient.onClick.AddListener(() =>
        {
            CheckName();
            OnClickStartClientButton();
        });

        // Add input field listener to update NetworkManager's Network Address
        // when changed.
        inputIP.onValueChanged.AddListener(delegate { OnNetworkAddressChange(); });
        inputPort.onValueChanged.AddListener(delegate { OnPortChange(); });
    }

    private void CheckName()
    {
        //Debug.Log($"CheckName is called value: {inputName.text}");
        if (string.IsNullOrEmpty(inputName.text))
        {
            LocalDataHolder.Instance.SetNickname("Player");
        }
        else
        {
            LocalDataHolder.Instance.SetNickname(inputName.text);

        }
    }

    #region Networking
    private void OnClickStartServerButton()
    {
        NetworkManager.singleton.StartServer();
    }
    private void OnClickStartHostButton()
    {
        NetworkManager.singleton.StartHost();
        SceneDataHolder.Instance.SetSaveID(selectedSaveID);
    }
    private void OnClickStartClientButton()
    {
        NetworkManager.singleton.StartClient();
    }

    private void OnNetworkAddressChange()
    {
        NetworkManager.singleton.networkAddress = inputIP.text;
    }

    private void OnPortChange()
    {
        SetPort(inputPort.text);
    }
    private void SetPort(string _port)
    {
        // only show a port field if we have a port transport
        // we can't have "IP:PORT" in the address field since this only
        // works for IPV4:PORT.
        // for IPV6:PORT it would be misleading since IPV6 contains ":":
        // 2001:0db8:0000:0000:0000:ff00:0042:8329
        if (Transport.active is PortTransport portTransport)
        {
            // use TryParse in case someone tries to enter non-numeric characters
            if (ushort.TryParse(_port, out ushort port))
                portTransport.Port = port;
        }
    }

    private void GetPort()
    {
        if (Transport.active is PortTransport portTransport)
        {
            inputPort.text = portTransport.Port.ToString();
        }
    }

    #endregion

}
