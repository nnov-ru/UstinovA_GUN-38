using UnityEngine;
using Zenject;

namespace Unity3D
{
    public class SingleSharedData : ISharedData
    {
        private readonly SignalBus _signal;
        private bool _locked;
        private GameEvent _event;
        private GameStatus _status;
        //private Team _currentPlayerTeam;
        private Unit _selectedUnit;
        private Cell _coloredCell;
        public Team White { get; private set; }
        public Team Black { get; private set; }

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked == value) return;
                _locked = value;
                _signal.Fire(_locked
                    ? GameStatus.Locked
                    : GameStatus.Unlocked);
            }
        }
        public GameEvent Event
        {
            get => _event;
            set
            {
                Debug.Log(_event == value
                    ? $"{nameof(GameEvent)}: repeated event: {value}"
                    : $"{nameof(GameEvent)}: new event: {value}");
                _event = value;
                _signal?.Fire(value);
            }
        }
        public GameStatus Status
        {
            get => _status;
            set
            {
                Debug.Log(_status == value
                    ? $"{nameof(GameStatus)}: repeated status: {value}"
                    : $"{nameof(GameStatus)}: new status: {value}");
                _status = value;
                _signal.Fire(value);
            }
        }
        public Cell Destination { get; set; }
        public Unit Target { get; set; }
        public Unit SelectedUnit
        {
            get=> _selectedUnit;
            set
            {
                _selectedUnit = value;
            }
        }
        //public Team CurrentPlayerTeam
        //{
        //    get => _currentPlayerTeam;
        //    set
        //    {
        //        _currentPlayerTeam = value;
        //    }
        //}
        [Inject]
        public SingleSharedData(SignalBus signal)
        {
            _signal = signal;
        }
    }
}