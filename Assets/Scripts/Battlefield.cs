using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Zenject;
using System.Linq;
//реализует логику подсветки клеток,
//проводит первичную инициализацию (выстраивая граф поля и связывая клетки с фишками/фигурами),
//кроме того может предоставлять API для обнаружения соседних клеток (если эта логика не будет реализована внутри класса Cell) 
namespace Unity3D
{
    public class Battlefield : IDisposable
    {
        private IGameplayCommand _command;
        private ITurn _turn;
        private readonly CellPaletteSettings _paletteSettings; //injected
        private readonly ISharedData _data;                     //injected
        private readonly SignalBus _signal;                     //injected

        private readonly Dictionary<NeighbourCell, Cell> _neighbours;
        private readonly Cell[] _cells;
        private readonly Unit[] _units;

        public event Action<Cell> OnCellClicked;
        [Inject]
        private void InjectCommands(IGameplayCommand command, ITurn turn)
        {
            _command = command;
            _turn = turn;
        }
        private void Callback()
        {
            foreach (var cell in _cells)
                cell.ResetSelect();
            switch (_data.Status)
            {
                case GameStatus.Selecting:
                    HandleSelecting();
                    break;
                case GameStatus.Motion:
                    HandleMotion();
                    break;
                case GameStatus.Attacking:
                    if (_data.Destination)
                        HandleAttacking();
                    break;
                case GameStatus.Confirmed:
                    HandleConfirming();
                    break;
                case GameStatus.Unlocked:
                case GameStatus.Locked:
                    break;
            }
        }
        private void HandleSelecting()
        {
            if (_data.SelectedUnit != null)
            {
                _data.SelectedUnit.Cell.SetSelect(_paletteSettings.SelectedCell);

                var availableCells = _command.Variants;
                foreach (var cell in availableCells)
                {
                    cell.SetSelect(_paletteSettings.MoveToCell);
                }
            }
            else
            {
                Debug.Log($"no selected unit - no game");
            }
        }
        private void HandleMotion()
        {
            if (_data.SelectedUnit != null)
            {
                _data.SelectedUnit.Cell.SetSelect(_paletteSettings.SelectedCell);
                
                if (_data.Destination != null)
                {
                    _data.Destination.SetSelect(_paletteSettings.SelectedCell);
                }
            }
        }
        private void HandleAttacking()
        {
            if (_data.SelectedUnit != null)
            {
                _data.SelectedUnit.Cell.SetSelect(_paletteSettings.SelectedCell);
                if (_data.Destination != null)
                    _data.Destination.SetSelect(_paletteSettings.SelectedCell);
                if (_data.Target.Cell != null && _data.Target.Team != _turn.Current)
                {
                    _data.Target.Cell.SetSelect(_paletteSettings.AttackCell);
                }
            }
        }
        private void HandleConfirming()
        {
            foreach (var cell in _cells)
                cell.ResetSelect();
        }

        public Battlefield(SignalBus signal, ISharedData data, CellPaletteSettings paletteSettings)
        {
            (_signal, _data, _paletteSettings) = (signal, data, paletteSettings);
            _signal.Subscribe<GameStatus>(Callback);

            #region What means to stand on a cell
            _cells = Object.FindObjectsOfType<Cell>();
            _units = Object.FindObjectsOfType<Unit>();
            foreach (var unit in _units)
            {
                Cell closestCell = null;
                float closestDistance = float.MaxValue;
                foreach (var cell in _cells)
                {
                    var distance = Vector3.Distance(unit.transform.position, cell.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCell = cell;
                    }
                }
                if (closestCell != null)
                {
                    unit.Cell = closestCell;
                    closestCell.Unit = unit;
                }
                else
                {
                    Debug.Log($"Couldnt find cell for {unit.name}");
                }
            }
            int unitsWithCells = _units.Count(unit  => unit.Cell != null);
            #endregion
            #region Directions from a cell
            _neighbours = new Dictionary<NeighbourCell, Cell>(_cells.Length * 8);

            var positions = Array.ConvertAll(_cells, t => t.transform.position);
            //var distance = 0f;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            foreach (var cell in _cells)
            {
                var pos = cell.transform.position;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minZ = Mathf.Min(minZ, pos.z);
                maxZ = Mathf.Max(maxZ, pos.z);
            }
            foreach (var cell in _cells)
            {
                var pos = cell.transform.position;
                int x = Mathf.RoundToInt(Mathf.InverseLerp(minX, maxX, pos.x) * 7);
                int y = Mathf.RoundToInt(Mathf.InverseLerp(minZ, maxZ, pos.z) * 7);
                cell.GridPosition = new Vector2Int(x, y);
            }
            for (int i = 0; i < _cells.Length; i++)
            {
                for (int j = 0; j < _cells.Length; j++)
                {
                    if (i == j) continue;

                    var source = positions[i];
                    var destination = positions[j];
                    var forward = destination.z.CompareTo(source.z);
                    var right = destination.x.CompareTo(source.x);

                    var type = (forward, right) switch
                    {
                        (1, 1) => NeighbourType.ForwardRight,
                        (1, 0) => NeighbourType.Forward,
                        (1, -1) => NeighbourType.ForwardLeft,
                        (0, 1) => NeighbourType.Right,
                        (0, -1) => NeighbourType.Left,
                        (-1, 1) => NeighbourType.BackRight,
                        (-1, 0) => NeighbourType.Back,
                        (-1, -1) => NeighbourType.BackLeft,
                        _ => default
                    };
                    var key = new NeighbourCell(type, _cells[i]);
                    var distance = Vector3.Distance(source, destination);
                    if (!_neighbours.TryGetValue(key, out var cell) ||
                        distance < Vector3.Distance(source, cell.transform.position))
                    {
                        _neighbours[key] = _cells[j];
                    }
                }
            }
        #endregion
            foreach (var cell in _cells)
            {
                cell.OnPointerClickEvent += OnClick;
#if UNITY_EDITOR
                cell.OnPointerClickEvent += DebugOnPointerClick;
#endif
            }
        }
        public bool TryGet(Cell source, NeighbourType type, out Cell cell)
        {
            var data = new NeighbourCell(type, source);
            return _neighbours.TryGetValue(data, out cell);
        }
        public Cell GetCell(int x, int y)
        {
            foreach (var cell in _cells)
            {
                if (cell.GridPosition.x == x && cell.GridPosition.y == y)
                    return cell;
            }
            return null;
        }

        private void OnClick(Cell cell)
        {
            OnCellClicked?.Invoke(cell);
        }
#if UNITY_EDITOR
        private void DebugOnPointerClick (Cell cell)
        {
            var start = cell.transform.position;
            Debug.DrawLine(start, start + Vector3.up * 5f, Color.green, 5f);
            foreach (var it in EnumUtility.All)
            {
                var key = new NeighbourCell(it, cell);
                if (!_neighbours.TryGetValue(key, out var neighbouring)) continue;

                start = neighbouring.transform.position;
                Debug.DrawLine(start, start + Vector3.up * 5f, Color.red, 5f);
            }
            Debug.Log($"Cell clicked at coordinates: {cell.GridPosition}");
        }
#endif
        private readonly struct NeighbourCell: IEquatable<NeighbourCell>
        {
            private readonly Cell _value;
            private readonly NeighbourType _type;
            public NeighbourCell(NeighbourType type, Cell value)
                => (_type, _value) = (type, value);
            public bool Equals(NeighbourCell other)
                => _type == other._type && Equals(_value, other._value);
            public override bool Equals(object obj)
            => obj is NeighbourCell other && Equals(other);
            public override int GetHashCode()
            => unchecked(HashCode.Combine(_type, _value) - 13);
        }
        public void Dispose()
        {
            if ( _cells != null )
            {
                foreach (var cell in _cells)
                {
                    if (cell != null)
                    {
                        cell.OnPointerClickEvent -= OnClick;
#if UNITY_EDITOR
                        cell.OnPointerClickEvent -= DebugOnPointerClick;
#endif
                    }
                }
            }
        }
        private void OnDestroy()
        {
            _signal?.TryUnsubscribe<GameEvent>(Callback);
        }

    }
}

