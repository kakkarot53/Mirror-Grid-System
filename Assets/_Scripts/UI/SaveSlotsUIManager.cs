using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveSlotsUIManager : MonoBehaviour
{
    [Header("Main Menu Items")]
    public GameObject mainMenuPanel;
    public Button createSaveBtn;
    public Button continueLastSaveBtn;
    public Button loadSaveBtn;
    public Button multiplayerBtn;

    [Header("Create New Save Items")]
    public GameObject createNewSavePanel;
    public Button createNewSaveBtn;
    public TMP_InputField saveName;
    public Button backFromNewSaveBtn;

    [Header("Load Save Items")]
    public GameObject loadSavePanel;
    public SaveSlot[] saveSlots;
    public Button backFromLoadSaveBtn;

    [Header("Confirmation Panel")]
    [SerializeField] private ConfirmPanel confirmationPopup;

    [Header("Multiplayer Panel")]
    public GameObject multiplayerPanel;
    public Button backFromMultiplayerBtn;


    //to know if the script is loading game or making a new game
    //false = new game, true = load game
    private bool isLoadingGame = false;

    private void Start()
    {
        //make sure that the main menu is activated first
        ActivatePanel(mainMenuPanel.name);

        //basic panel switchies buttons
        createSaveBtn.onClick.AddListener(()=> ActivateMenu(false));
        continueLastSaveBtn.onClick.AddListener(() => SaveGameAndLoadScene());
        loadSaveBtn.onClick.AddListener(()=> ActivateMenu(true));
        multiplayerBtn.onClick.AddListener(() => ActivatePanel(multiplayerPanel.name));

        backFromNewSaveBtn.onClick.AddListener(()=>ActivatePanel(mainMenuPanel.name));
        backFromLoadSaveBtn.onClick.AddListener(()=>ActivatePanel(mainMenuPanel.name));
        backFromMultiplayerBtn.onClick.AddListener(() => ActivatePanel(mainMenuPanel.name));

        DisableButtonsDependingOnData();

        foreach(SaveSlot saveslot in saveSlots)
        {
            saveslot.loadSaveBtn.onClick.AddListener(() => OnSaveSlotClicked(saveslot));
            saveslot.deleteSaveBtn.onClick.AddListener(() => OnClearClicked(saveslot));
        }
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        ActivatePanel(loadSavePanel.name);
        // set mode
        this.isLoadingGame = isLoadingGame;

        // load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData _profileData;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out _profileData);
            saveSlot.SetData(_profileData);
            if (_profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // disable all buttons
        DisableMenuButtons();

        // case - loading game
        if (isLoadingGame)
        {
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene();
        }
        // case - new game, but the save slot has data
        else if (saveSlot.hasData)
        {
            confirmationPopup.ActivateMenu(
                "Starting a New Game with this slot will override the currently saved data. Are you sure?",
                // function to execute if we select 'yes'
                () => {
                    DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    //trigger rename here
                    SetNamePanel((name) => {
                        DataPersistenceManager.instance.NewGame(name);
                        SaveGameAndLoadScene();
                    });

                },
                // function to execute if we select 'cancel'
                () => {
                    this.ActivateMenu(isLoadingGame);
                }
            );
        }
        // case - new game, and the save slot has no data
        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            //trigger rename here
            SetNamePanel((name) => {
                DataPersistenceManager.instance.NewGame();
                SaveGameAndLoadScene();
            });
        }
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopup.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            // function to execute if we select 'yes'
            () => {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            // function to execute if we select 'cancel'
            () => {
                ActivateMenu(isLoadingGame);
            }
        );
    }

    private void ActivatePanel(string name)
    {
        mainMenuPanel.SetActive(mainMenuPanel.name == name);
        loadSavePanel.SetActive(loadSavePanel.name == name);
        multiplayerPanel.SetActive(multiplayerPanel.name == name);
    }

    private void SaveGameAndLoadScene()
    {
        // save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();
        // load the next scene - which will in turn load the game because of 
        // OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync(1);
    }
    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueLastSaveBtn.interactable = false;
            loadSaveBtn.interactable = false;
        }
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
    }

    private void SetNamePanel(System.Action<string> onNameSet)
    {
        createNewSavePanel.SetActive(true);

        createNewSaveBtn.onClick.RemoveAllListeners(); // Prevent duplicate listeners
        createNewSaveBtn.onClick.AddListener(() =>
        {
            createNewSavePanel.SetActive(false);
            onNameSet?.Invoke(saveName.text); // Pass the name back via callback
        });

        backFromNewSaveBtn.onClick.RemoveAllListeners();
        backFromNewSaveBtn.onClick.AddListener(() => createNewSavePanel.SetActive(false));
    }
}
