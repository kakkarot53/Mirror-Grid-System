using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] 
    private string profileId="";

    public Button loadSaveBtn;
    public Button deleteSaveBtn;

    [SerializeField] private GameObject hasDataPanel;
    [SerializeField] private GameObject noDataPanel;

    [SerializeField] private TMP_Text saveNameTxt;
    [SerializeField] private TMP_Text lastSavedTxt;

    public bool hasData { get; private set; } = false;

    public void SetData(GameData data)
    {
        // there's no data for this profileId
        if (data == null)
        {
            hasData = false;
            noDataPanel.SetActive(true);
            hasDataPanel.SetActive(false);
            deleteSaveBtn.gameObject.SetActive(false);
        }
        // there is data for this profileId
        else
        {
            hasData = true;
            noDataPanel.SetActive(false);
            hasDataPanel.SetActive(true);
            deleteSaveBtn.gameObject.SetActive(true);

            saveNameTxt.text = data.name;
            lastSavedTxt.text = "Last Edited: " + DateTime.FromBinary(data.lastUpdated).ToString("dd/MM/yyyy HH:mm");
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        loadSaveBtn.interactable = interactable;
        deleteSaveBtn.interactable = interactable;
    }
}
