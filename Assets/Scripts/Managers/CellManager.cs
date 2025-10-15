using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity3D
{
    public class CellManager : MonoBehaviour
    {
        private Dictionary<NeighbourCell, Cell> _neighbours;
        private Cell[] _cells;
        private Unit[] _units;

        public event Action<Cell> OnCellClicked;
        private void Awake()
        {
            _cells = FindObjectsOfType<Cell>();
            _units = FindObjectsOfType<Unit>();
            _neighbours = new Dictionary<NeighbourCell, Cell>(_cells.Length * 8);

            var positions = Array.ConvertAll(_cells, t => t.transform.position);
            //var distance = 0f;
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
            }
            foreach (var cell in _cells)
            {
                cell.OnPointerClickEvent += OnClick;
#if UNITY_EDITOR
                cell.OnPointerClickEvent += DebugOnPointerClick;
#endif
            }
        }
        private void OnClick(Cell cell)
        {
            OnCellClicked?.Invoke(cell);
        }
#if UNITY_EDITOR
        private void DebugOnPointerClick (Cell cell)
        {
            Debug.Log($"Cell clicked at coordinates: {cell.transform.position}");
        }
#endif
        private void OnDestroy()
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
    }
}

