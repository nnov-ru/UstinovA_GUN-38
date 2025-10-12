using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SceneController>().AsSingle();
        Container.Bind<SingleController>().AsSingle();
        Container.BindInterfacesTo<MultiplayerController>().AsSingle();
    }

    public class SingleController : IController { }

    public class MultiplayerController : IController { }

    public interface IController
    {
        
    }
}
