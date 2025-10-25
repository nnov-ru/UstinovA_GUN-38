using Zenject;
using System.Collections;
using UnityEngine;
using System;
//блокирует управление на время визуализации хода,
//а также занимается визуализацией хода
//и обработкой результатов 
namespace Unity3D
{
    public class PlayerController : MonoBehaviour
    {
        private SignalBus _signal; //injected
        private ISharedData _data; //injected

        public event Action<Unit, Cell, Cell> OnMoveEndCallback;
        [Inject]
        private void Construct(SignalBus signal, ISharedData data)
        {
            (_signal, _data) = (signal, data);
            _signal.Subscribe<GameEvent>(StartPlay);
        }
        private void StartPlay(GameEvent arg)
        {
            //confirm target for selected unit
            if (arg is not GameEvent.Confirm) return;
            if (_data.Status is not GameStatus.Confirmed) return;
            if (_data.SelectedUnit == null || _data.Destination == null)
                return;
            Move(_data.SelectedUnit,_data.Destination);
            //else
            //{
            //    var target = _data.Target.Unit;
            //    target.Health -= destination.Settings.Stats.Damage;
            //    if (target.Health <= 0)
            //    {
            //        _data.Target.Unit = null;
            //        Destroy(target.gameObject);
            //    }
            //    _data.Target = null;
            //    _data.Status = GameStatus.Unlocked;
            //}
            //checker's targets are always empty
        }
        //private void OnEndPlay()
        //{
        //    _data.Status = GameStatus.Unlocked;
        //    _data.Destination.OnMoveEndCallback -= OnEndPlay;
        //if (_data.Destination.Settings.Mobility.MoveAndAttackInTurn)
        //{
        //    _data.Target = null;
        //    _data.Status = GameStatus.Attack;
        //}
        //to destroy eaten checker before unlock !!
        //to continue move if there is another checker to eat until there's none
        //}
        public void Move(Unit unit, Cell cell)
        {
            if (unit.IsMoving || cell == null || cell == unit.Cell || unit == null)
                return;
            StartCoroutine(ParabolaCoroutine(unit, cell));
        }
        private IEnumerator ParabolaCoroutine(Unit unit, Cell cell)
        {
            unit.IsMoving = true;
            Cell startCell = unit.Cell;

            Unit attackedUnit = _data.Target;
            Cell attackedCell = null;

            if (attackedUnit != null)
                attackedCell = attackedUnit.Cell;

            Vector3 start = unit.transform.position;
            Vector3 end = cell.transform.position;
            end.y = start.y;

            float time = Vector3.Distance(start, end) / unit.Speed;
            float delta = 0f;

            while (delta < time)
            {
                delta += Time.deltaTime;
                float pathPart = delta / time;
                Vector3 horposition = Vector3.Lerp(start, end, pathPart);
                float height = -4f * unit.JumpHeight * (pathPart - 0.5f) * (pathPart - 0.5f) + unit.JumpHeight;
                unit.transform.position = new Vector3(horposition.x, start.y + Mathf.Max(0, height), horposition.z);
                yield return null;
            }
            unit.transform.position = end;

            if (attackedUnit != null && attackedCell != null)
            {
                StartCoroutine(UnitFallCoroutine(attackedUnit));
            }
            if (startCell != null && startCell.Unit == unit)
                startCell.Unit = null;
            unit.Cell = cell;
            cell.Unit = unit;
            unit.IsMoving = false;
            OnMoveEndCallback?.Invoke(unit, startCell, cell);
        }
        private IEnumerator UnitFallCoroutine(Unit unit)
        {
            if (unit == null) yield break;
            Vector3 start = unit.transform.position;
            Vector3 end = start + Vector3.down * 2f;

            float sinkTime = .5f;
            float delta = 0f;
            while (delta < sinkTime)
            {
                delta += Time.deltaTime;
                float pathPart = delta / sinkTime;
                unit.transform.position = Vector3.Lerp(start, end, pathPart);
                yield return null;
            }
            if (unit.Cell != null)
                unit.Cell.Unit = null;
            Destroy(unit.gameObject);
        }
        private void OnDestroy()
        {
            _signal?.TryUnsubscribe<GameEvent>(StartPlay);
        }
    }
}
