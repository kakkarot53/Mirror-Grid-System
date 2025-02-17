using UnityEngine;
using UnityEngine.UI;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_GridObjectSelectionOption : MonoBehaviour
    {
        public static event System.Action<GameObject> OnOptionSelected;

        [SerializeField] private GameObject _gridObjectToSpawnPrefab;
        [SerializeField] private Image _btnIcon;

        private PrefabDataObj data;
        public void BtnSetup(PrefabDataObj _data)
        {
            if (_data != null)
                data = _data;

            _gridObjectToSpawnPrefab = data.prefabToSpawn;

            if (_btnIcon != null)
            {
                _btnIcon.sprite = data.spriteToSpawn;
            }

            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(HandleButtonClicked);
            }
        }

        private void HandleButtonClicked()
        {
            if (_gridObjectToSpawnPrefab == null)
            {
                Debug.LogError("Error. No prefab assigned to spawn on this selection option");
            }

            GameObject objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());

            objectToPlace.name = data.name;

            if (!objectToPlace.TryGetComponent(out ExampleGridObject gridObject))
            {
                objectToPlace.AddComponent<ExampleGridObject>();
            }

            OnOptionSelected?.Invoke(objectToPlace);

            GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);

            if (SaveLoadManager.instance != null)
                SaveLoadManager.instance.objData.Add(objectToPlace);
        }
    }
}