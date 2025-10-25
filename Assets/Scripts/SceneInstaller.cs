using System.Linq;
using UnityEngine;
using Zenject;

namespace Unity3D
{
    public class SceneInstaller : MonoInstaller
    {
        private Controls _controls;

        [SerializeField]
        private InputManager _inputManager;
        [SerializeField]
        private CellPaletteSettings _cellPaletteSettings;
        //[SerializeField]
        //private TurnPanelSettings _turnPanelSettings;
        [SerializeField]
        private UnitGameSettings _unitGameSettings;
        [SerializeField]
        private TurnIndicator _turnIndicator;
        public override void InstallBindings()
        {
            _controls = new Controls();
            _controls.Game.Enable();
            Container.BindInstance(_controls.Game).AsSingle();
            Container.BindInterfacesAndSelfTo<Battlefield>().AsSingle();
            Container.BindInstance(_cellPaletteSettings).AsSingle();

            //Container.BindInstance(_turnPanelSettings).AsSingle();
            Container.BindInstance(_unitGameSettings).AsSingle();
            Container.BindInstance(_turnIndicator).AsSingle();
            Container.Bind<InputManager>().FromComponentInHierarchy().AsSingle();

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameEvent>();
            Container.DeclareSignal<GameStatus>();

            var units = FindObjectsOfType<Unit>();
            Container.BindInstance(units).AsSingle();
            var teams = units.Select(t => t.Team).Distinct().ToList();
            teams.Sort();
            Container.Bind<ITurn>().To<OneByOneTurn>().AsSingle().WithArguments(teams);
            Container.Bind<ISharedData>().To<SingleSharedData>().AsSingle();
            Container.Bind<IGameplayCommand>().To<CheckersCommand>().AsSingle();
            Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        }
        private void OnDestroy()
        {
            _controls?.Dispose();
        }
    } 
}
