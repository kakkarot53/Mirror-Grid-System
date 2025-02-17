using UnityEngine;
using UnityEngine.UI;
using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.Models;
using Hypertonic.GridPlacement;
using Unity.Cinemachine;

public class MultipleGrids : MonoBehaviour
{
    [SerializeField]
    private GridSetupForMultipleGrids[] setup;

    private void Awake()
    {
        CreateGridManagers();
    }

    private void OnEnable()
    {
        foreach (GridSetupForMultipleGrids grid in setup)
        {
            grid._selectGridButton.onClick.AddListener(()=>
            {
                HandleButtonPressed(grid);
            });
        }
        ExampleGridObject.OnObjectSelected += HandleExampleGridObjectSelected;
    }

    private void OnDisable()
    {
        foreach (GridSetupForMultipleGrids grid in setup)
        {
            grid._selectGridButton.onClick.RemoveListener(() =>
            {
                HandleButtonPressed(grid);
            });
        }
        ExampleGridObject.OnObjectSelected -= HandleExampleGridObjectSelected;
    }

    private void CreateGridManagers()
    {
        int i = 0;
        foreach(GridSetupForMultipleGrids grid in setup)
        {
            GameObject gridMngObj = new GameObject($"Grid Manager {i}");
            GridManager gridMng = gridMngObj.AddComponent<GridManager>();
            gridMng.Setup(grid._gridSettings);
            i++;
        }
    }

    private void HandleButtonPressed(GridSetupForMultipleGrids grid)
    {
        foreach (GridSetupForMultipleGrids g in setup)
        {
            if (g.Equals(grid))
            {
                g._camPos.Priority = 1;
            }
            else
            {
                g._camPos.Priority = -1;
            }
        }

        GridManagerAccessor.SetSelectedGridManager(grid._gridSettings.Key);
        
    }

    private void HandleExampleGridObjectSelected(GameObject gameObject)
    {
        string gridKeyOfSelectedObject = gameObject.GetComponent<GridObjectInfo>().GridKey;
        GridManagerAccessor.SetSelectedGridManager(gridKeyOfSelectedObject);
    }
}

[System.Serializable]
public struct GridSetupForMultipleGrids
{
    public GridSettings _gridSettings;
    public Button _selectGridButton;

    //camera related stuff
    public CinemachineCamera _camPos;
}
