using UnityEngine;
using Zenject;

namespace Unity3D
{
    public class GameInstaller : MonoInstaller
    {
        private Controls _controls;
        private Unit _selectedUnit;

        [SerializeField]
        private CellManager _cellManager;
        [SerializeField]
        private SceneController _sceneController;
        [SerializeField]
        private CellPaletteSettings _cellPaletteSettings;
        public override void InstallBindings()
        {
            _controls = new Controls();
            _controls.Game.Enable();
            Container.BindInstance(_controls.Game).AsSingle();
            Container.BindInstance(_cellManager).AsSingle();
            Container.BindInstance(_sceneController).AsSingle();
            Container.BindInstance(_cellPaletteSettings).AsSingle();

            _cellManager.OnCellClicked += CellManagerOnCellClicked;
        }

        private void CellManagerOnCellClicked(Cell obj)
        {
            obj.SetSelect(_cellPaletteSettings.SelectedCell);
            if (obj.Unit != null)
            {
                if (_selectedUnit != null)
                {
                    _selectedUnit.Cell.ResetSelect();
                }
                _selectedUnit = obj.Unit;
                obj.SetSelect(_cellPaletteSettings.SelectedCell);
            }
            else if (_selectedUnit != null && obj.Unit == null)
            {
                _selectedUnit.Cell.ResetSelect();
                _selectedUnit.OnMoveEndCallback += OnUnitMoveEnd;
                _selectedUnit.Move(obj);
                _selectedUnit = null;
            }
        }
        private void OnUnitMoveEnd(Unit unit)
        {
            unit.OnMoveEndCallback -= OnUnitMoveEnd;
            unit.Cell.ResetSelect();
        }
        private void OnDestroy()
        {
            if ( _cellManager != null )
            {
                _cellManager.OnCellClicked -= CellManagerOnCellClicked;
            }
            _controls.Dispose();
        }
    } 
}
