using System.Collections.Generic;
using Zenject;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;
// Interact должна обрабатывать взаимодействие игрока с игровым полем и регистрировать действия 
namespace Unity3D
{
    public class CheckersCommand : IGameplayCommand
    {
        private ISharedData _data;          //injected
        private ITurn _turn;          //injected
        private Battlefield _cellManager; //injected
        private PlayerController _playerController;//injected
        private CellPaletteSettings _cellPaletteSettings;//injected
        private Unit _attackingUnit;

        private readonly HashSet<Cell> _availableCells = new (13);
        private readonly HashSet<Cell> _availableAttacks = new (4);

        [Inject]
        public CheckersCommand(ISharedData data, ITurn turn, Battlefield cellManager, PlayerController playerController, CellPaletteSettings cellPaletteSettings)
        {
            _data = data;
            _turn = turn;
            _cellManager = cellManager;
            _playerController = playerController;
            _cellPaletteSettings = cellPaletteSettings;

            _playerController.OnMoveEndCallback += OnUnitMoveEnd;
        }
        public IEnumerable<Cell> Variants => _availableCells;
        public void Interact(Cell cell)
        {
            if (cell.Unit != null && cell.Unit == _data.SelectedUnit)
            {
                Deselect();
                return;
            }
            if (cell.Unit != null && cell.Unit.Team == _turn.Current)
            {
                Deselect();
                _data.SelectedUnit = cell.Unit;
                FindAvailableCells(cell.Unit);
                _data.Event = GameEvent.Select;
                _data.Status = GameStatus.Selecting;
                return;
            }
            else if (cell.Unit != null)
            {
                Debug.Log($"it's {_turn.Current}'s turn, you can't touch other player's stuff");
            }
            if (cell.Unit == null && _data.SelectedUnit != null && _availableCells.Contains(cell))
            {
                _data.Destination = cell;
                Unit attackedUnit = FindAttackedUnit(cell);
                bool isAttacking = attackedUnit != null;
                _data.Target = isAttacking ? attackedUnit : null;
                _data.Status = isAttacking ? GameStatus.Attacking : GameStatus.Motion;
                return;
            }
            Deselect();
        }
        private void Deselect() //changing, second click or nowhere
        {
            if (_data.SelectedUnit != null)
            {
                _data.Status = GameStatus.Unlocked;
                _data.SelectedUnit.Cell.ResetSelect();
                foreach (var availableCell in _availableCells)
                {
                    availableCell.ResetSelect();
                }
                _availableCells.Clear();
                _data.SelectedUnit = null;
                _data.Target = null;
                _data.Destination = null;
            }
            if (_data.Target != null) _data.Target = null;
        }
        private void FindAvailableCells(Unit unit)
        {
            _availableCells.Clear();
            _availableAttacks.Clear();

            if (_attackingUnit != null && unit != _attackingUnit)
                return;
            var moveDirections = GetMoveDirections(unit);
            foreach (var direction in moveDirections)
            {
                CheckMoveDirection(unit, unit.IsQueen, direction);
            }
            var attackDirections = GetAttackDirections(unit);
            foreach (var direction in attackDirections)
            {
                CheckAttackDirection(unit, unit.IsQueen, direction);
            }

            bool globalAttacks = ThereAreAnyAttacks(_turn.Current);
            if (globalAttacks)
            {
                _availableCells.Clear();
                foreach (var attackCell in _availableAttacks)
                    _availableCells.Add(attackCell);
            }
            else
            {
                if (_availableAttacks.Count > 0)
                {
                    _availableCells.Clear();
                    foreach (var attackCell in _availableAttacks)
                    {
                        _availableCells.Add(attackCell);
                    }
                    Debug.Log($"Finished: found only attacks: {_availableCells.Count} attack(s)");
                }
                else
                    Debug.Log($"Finished: found {_availableCells.Count} moves");
            }
        }
        private NeighbourType[] GetMoveDirections(Unit unit)
        {
            if (unit.IsQueen)
            {
                return GetAttackDirections(unit);
            }
            else
            {
                if (_turn is OneByOneTurn oneByOneTurn)
                {
                    bool isMovingRight = unit.Team == oneByOneTurn.White;
                    return isMovingRight
                        ? new NeighbourType[] { NeighbourType.ForwardLeft, NeighbourType.BackLeft }
                        : new NeighbourType[] { NeighbourType.ForwardRight, NeighbourType.BackRight };
                }
                return new NeighbourType[0];
            }
        }
        private NeighbourType[] GetAttackDirections(Unit unit)
        {
            return new NeighbourType[]
                {
                    NeighbourType.ForwardLeft, NeighbourType.ForwardRight,
                    NeighbourType.BackLeft, NeighbourType.BackRight
                };
        }
        private void CheckMoveDirection(Unit unit, bool isQueen, NeighbourType direction)
        {
            if (!_cellManager.TryGet(unit.Cell, direction, out Cell nextCell))
                return;
            if (isQueen)
            {
                while (nextCell != null && nextCell.Unit == null)
                {
                    _availableCells.Add(nextCell);
                    if (!_cellManager.TryGet(nextCell, direction, out nextCell))
                        break;
                }
            }
            else
            {
                if (nextCell.Unit == null)
                {
                    _availableCells.Add(nextCell);
                }
            }
        }
        private void CheckAttackDirection(Unit unit, bool isQueen, NeighbourType direction)
        {
            if (isQueen)
            {
                Cell currentCell = unit.Cell;
                Cell enemyCell = null;
                while(_cellManager.TryGet(currentCell, direction, out Cell nextCell))
                {
                    if (nextCell == null) break;
                    currentCell = nextCell;
                    if (currentCell.Unit != null)
                    {
                        if (currentCell.Unit.Team == unit.Team) 
                            return;
                        else
                        {
                            enemyCell = currentCell;
                            break;
                        }
                    }
                }
                if (enemyCell == null) return;
                Cell checkCell = enemyCell;
                bool foundFreeCell = false;
                while (_cellManager.TryGet(checkCell, direction, out Cell destinationCell))
                {
                    if (destinationCell == null) break;
                    if (destinationCell.Unit != null) break;
                    foundFreeCell = true;
                    _availableAttacks.Add(destinationCell);
                    checkCell = destinationCell;
                }
            }
            else
            {
                if (!_cellManager.TryGet(unit.Cell, direction, out Cell enemyCell))
                    return;
                if (enemyCell.Unit == null || enemyCell.Unit.Team == unit.Team)
                    return;
                if (!_cellManager.TryGet(enemyCell, direction, out Cell destinationCell))
                    return;
                if (destinationCell.Unit == null)
                {
                    _availableAttacks.Add(destinationCell);
                }
            }
        }
        private bool ThereAreAnyAttacks(Team playerTeam)
        {
            Unit[] allUnits = Object.FindObjectsOfType<Unit>();
            foreach (Unit globalUnit in allUnits)
            {
                if (globalUnit.Team == playerTeam && globalUnit.Cell != null)
                {
                    var attackDir = GetAttackDirections(globalUnit);
                    foreach (var dir in attackDir)
                    {
                        if (ThereIsAttackDirection(globalUnit, dir))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool ThereAreUnitAttacks(Unit selectedUnit)
        {
            var attackDir = GetAttackDirections(selectedUnit);
            foreach (var dir in attackDir)
            {
                if (ThereIsAttackDirection(selectedUnit, dir))
                {
                    return true;
                }
            }
            return false;
        }
        private bool ThereIsAttackDirection(Unit unit, NeighbourType dir)
        {
            if (!_cellManager.TryGet(unit.Cell, dir, out Cell enemyCell))
                return false;
            if (enemyCell?.Unit == null || enemyCell.Unit.Team == unit.Team)
                return false;
            if (!_cellManager.TryGet(enemyCell, dir, out Cell destinationCell))
                return false;
            return destinationCell?.Unit == null;
        }
        private Unit FindAttackedUnit(Cell destinationCell)
        {
            if (_data.SelectedUnit == null) return null;
            if (_data.SelectedUnit.IsQueen)
            {
                Vector2Int startPos = _data.SelectedUnit.Cell.GridPosition;
                Vector2Int destPos = destinationCell.GridPosition;
                Vector2Int direction = new Vector2Int(
                    destPos.x.CompareTo(startPos.x),
                    destPos.y.CompareTo(startPos.y)
                    );
                Vector2Int currentPos = startPos + direction;
                Unit foundEnemy = null;
                while (currentPos != destPos)
                {
                    Cell currentCell = _cellManager.GetCell(currentPos.x, currentPos.y);
                    if (currentCell == null) break;
                    if (currentCell.Unit != null)
                    {
                        if (currentCell.Unit.Team == _data.SelectedUnit.Team)
                            return null;
                        if (foundEnemy == null)
                            foundEnemy = currentCell.Unit;
                        else
                            return null;
                    }
                    currentPos += direction;
                }
                return foundEnemy;
            }
            else
            {
                Vector2Int currentPos = _data.SelectedUnit.Cell.GridPosition;
                Vector2Int destPos = destinationCell.GridPosition;
                if (Mathf.Abs(destPos.x - currentPos.x) != 2 || 
                    Mathf.Abs(destPos.y - currentPos.y) != 2) 
                    return null;

                Vector2Int direction = new Vector2Int(
                    (destPos.x - currentPos.x) / 2,
                    (destPos.y - currentPos.y) / 2
                    );
                Vector2Int attackedPos = currentPos + direction;
                Cell attackedCell = _cellManager.GetCell(attackedPos.x, attackedPos.y);
                if (attackedCell?.Unit != null && attackedCell.Unit.Team != _data.SelectedUnit.Team)
                {
                    Debug.Log($"Target is found at {attackedPos}: {attackedCell.Unit.Team}");
                    return attackedCell.Unit;
                }
                Debug.Log($"Target not found at {attackedPos}");
                return null;
            }
        }
        private void OnUnitMoveEnd(Unit unit, Cell fromCell, Cell toCell)
        {
            CheckPromotion(unit, toCell);
            if (_data.Target != null )
            {
                _data.Target.Cell.Unit = null;
            }
            if (_data.Target != null && ThereAreUnitAttacks(unit))
            {
                _attackingUnit = unit;
                _data.Target = null;
                _data.Destination = null;
                _availableCells.Clear();
                _availableAttacks.Clear();
            }
            else
            {
                //_data.CurrentPlayerTeam = _data.CurrentPlayerTeam == Team.Player1
                //    ? Team.Player2
                //    : Team.Player1;
                _attackingUnit = null;
                _data.Status = GameStatus.Locked;// не сразу, еще следующие атаки
                new WaitForSeconds(.5f);
                _data.Status = GameStatus.Unlocked;
                _data.SelectedUnit = null;
                _availableCells.Clear();
                _availableAttacks.Clear();
                _data.Target = null;
                _data.Destination = null;
            }
        }
        private void CheckPromotion(Unit unit, Cell cell)
        {
            bool isLastRow = false;
            if (_turn is OneByOneTurn oneByOneTurn)
            {
                if (unit.Team == oneByOneTurn.White)
                {
                    isLastRow = cell.GridPosition.x == 0;
                }
                else if (unit.Team == oneByOneTurn.Black)
                {
                    isLastRow = cell.GridPosition.x == 7;
                }
            }
            if (isLastRow && !unit.IsQueen)
                {
                    unit.Promotion();
                    Debug.Log($"{unit.name} Promoted to Queen!");
                }
        }
        public void Dispose()
        {
            _playerController.OnMoveEndCallback -= OnUnitMoveEnd;
        }
    }
}
