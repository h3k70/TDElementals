using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameInputMap>().FromNew().AsSingle();
        Container.Bind<Selector>().FromNew().AsSingle().NonLazy();
    }
}
