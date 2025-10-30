using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Reflection;

namespace Unity3D.Editor.Cheats
    {
    public class EditorCheatWindow : EditorWindow
    {
        private EditorControls _controls;
        private static ITurn _turn;

        [MenuItem("Unity3D/Editor Cheat Window", priority = 1)]
        public static void ShowWindow()
        {
            GetWindow<EditorCheatWindow>(false, "Editor Cheats", true);
        }
        private void OnEnable()
            => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        private void OnDisable()
            => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _controls = new EditorControls();
                    _controls.Cheats.Enable();
                    _controls.Cheats.NextTurn.performed += OnNextTurn;
                    _controls.Cheats.Kill.performed += OnKill;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    _controls.Disable();
                    _controls.Cheats.NextTurn.performed -= OnNextTurn;
                    _controls.Cheats.Kill.performed -= OnKill;
                    _controls.Dispose();
                    _controls = null;
                    break;
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                    break;
            }
        }
        private void OnNextTurn(InputAction.CallbackContext obj)
        {
            if (!FindControllerAndData(out var data))
                return;
            var type = data.GetType();
            var property = type.GetProperty(nameof(SingleSharedData.Event));
            property.SetValue(data, GameEvent.Empty);
        }
        private void OnKill(InputAction.CallbackContext obj)
        {
            if (!FindControllerAndData(out var data))
                return;

            var dataType = data.GetType();
            var property = dataType.GetProperty(nameof(SingleSharedData.Target));
            var target = property.GetValue(data) as Unit;
            if (target == null)
            {
                Debug.LogError("Can't destroy non selected target");
                return;
            }
            var selectedUnitProperty = dataType.GetProperty(nameof(SingleSharedData.SelectedUnit));
            var selectedUnit = selectedUnitProperty.GetValue(data) as Unit;
            if (selectedUnit != null && target.Team ==selectedUnit.Team)
            {
                Debug.LogError("Can't kill own unit");
                return;
            }
            if (target.Cell != null)
            {
                target.Cell.Unit = null;
                target.Cell.ResetSelect();
            }

            Destroy(target.gameObject);
            property.SetValue(data, null);
            var destProperty = dataType.GetProperty(nameof(SingleSharedData.Destination));
            destProperty.SetValue(data, null);
            if (selectedUnit != null)
            {
                selectedUnit.Cell.ResetSelect();
            }
            selectedUnitProperty.SetValue(data, null);
            var statusProperty = dataType.GetProperty(nameof(SingleSharedData.Status));
            statusProperty.SetValue(data, GameStatus.Unlocked);
        }
        private bool FindControllerAndData(out ISharedData data)
        {
            var controller = FindObjectOfType<BattleController>();
            if (controller == null)
            {
                Debug.LogError($"Can't find <b>{nameof(BattleController)}</b>");
                data = default;
                return false;
            }

            var type = controller.GetType();
            var field = type.GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);
            data = field.GetValue(controller) as ISharedData;
            if (data == null)
            {
                Debug.LogError($"Field <b>{nameof(BattleController)}._data</b> is null");
                return false;
            }

            return true;
        }
    }
}
