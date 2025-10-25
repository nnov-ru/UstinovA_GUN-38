using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
//manages actions from all input - former SceneController
//(или это может делать напрямую команда)
namespace Unity3D
{
    public class BattleController : MonoBehaviour
    {
        private IGameplayCommand _command;      //injected
        private ISharedData _data;              //injected
        private SignalBus _signal;              //injected
        private Controls.GameActions _controls; //injected.GameActions
        [Inject]
        private Battlefield _cellManager;       //injected
        //private CellPaletteSettings _cellPaletteSettings;
        private InputManager _inputManager;     //injected
        private PlayerController _playerController; //injected

        public Battlefield Battlefield => _cellManager;
        [Inject]
        private void Construct(PlayerController playerController, IGameplayCommand command, ISharedData data, SignalBus signal, Battlefield cellManager, Controls.GameActions controls, InputManager inputManager, ITurn iturn)
        {
            (_playerController, _command, _data, _signal, _cellManager, _controls, _inputManager) = (playerController, command, data, signal, cellManager, controls, inputManager);

            _cellManager.OnCellClicked += _command.Interact;
            _inputManager.OnCancelPressed += OnCancelHandler;
            _inputManager.OnRestartPerformed += OnRestartHandler;
            _inputManager.OnConfirmPressed += OnConfirmHandler;
            //_signal.Subscribe<GameEvent>(Callback);
            //SubscribeToCellEvents();
        }
        //private void Callback(GameEvent arg)
        //{
        //    if (arg is not GameEvent.Select) return; //ивент селект? уточнить зачем он
        //    switch (_data.Status)
        //    {
        //        case GameStatus.Selecting:
        //        case GameStatus.Motion:
        //        case GameStatus.Attacking:
        //        case GameStatus.Unlocked:
        //        case GameStatus.Locked:
        //        case GameStatus.Confirmed:
        //            break;
        //    }
        //}
        public void OpenMainScene()
        {
            throw new NotImplementedException();
            SceneManager.LoadScene(0);
        }
        public void OpenGameScene()
        {
            SceneManager.LoadScene(1//, LoadSceneMode.Additive);
                                    );
        }
        private void OnCancelHandler()
        {
            switch (_data.Status)
            {
                case GameStatus.Selecting:
                case GameStatus.Motion:
                case GameStatus.Attacking:
                    _data.SelectedUnit = null;
                    _data.Target = null;
                    _data.Destination = null;
                    _data.Event = GameEvent.Cancel;
                    _data.Status = GameStatus.Unlocked;
                    break;
                case GameStatus.Confirmed:
                case GameStatus.Locked:
                    Debug.Log("Cannot cancel during animation");
                    break;
            }
        }
        private void OnRestartHandler()
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
        private void OnConfirmHandler()
        {
            switch (_data.Status)
            {
                case GameStatus.Motion:
                case GameStatus.Attacking:
                    _data.Status = GameStatus.Confirmed;
                    _data.Event = GameEvent.Confirm;
                    break;
            }
        }
        private void OnDestroy()
        {
            if (_cellManager != null)
            {
                _cellManager.OnCellClicked -= _command.Interact;
            }
            if (_inputManager != null)
            {
                _inputManager.OnCancelPressed -= OnCancelHandler;
                _inputManager.OnConfirmPressed -= OnConfirmHandler;
                _inputManager.OnRestartPerformed -= OnRestartHandler;
            }
        }
    }
}
