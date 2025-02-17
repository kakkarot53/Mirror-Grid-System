using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SceneSlot : MonoBehaviour
{
    /*
     NOTE: this script is used ONLY as a data container for scene selection in multiplayer
     */

    [SerializeField]
    private string profileId = "";

    public Button selectBtn;

    [SerializeField] private GameObject hasDataPanel;
    [SerializeField] private GameObject noDataPanel;

    [SerializeField] private TMP_Text saveNameTxt;

    public bool hasData { get; private set; } = false;

    public void SetData(GameData data)
    {
        // there's no data for this profileId
        if (data == null)
        {
            hasData = false;
            noDataPanel.SetActive(true);
            hasDataPanel.SetActive(false);
        }
        // there is data for this profileId
        else
        {
            hasData = true;
            noDataPanel.SetActive(false);
            hasDataPanel.SetActive(true);

            saveNameTxt.text = data.name;
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        selectBtn.interactable = interactable;
    }
}
