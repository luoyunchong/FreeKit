using IGeekFan.FreeKit.Extras.Dependency;

namespace Module1.Domain
{
    public interface ITransientSongManager : ITransientDependency
    {

    }

    public class TransientSongManager : ITransientSongManager
    {

    }

    public interface ISingletonSongManager : ISingletonDependency
    {

    }

    public class SingletonSongManager : ISingletonSongManager
    {

    }

    public interface IScopedSongManager : IScopedDependency
    {

    }

    public class ScopedSongManager : IScopedSongManager
    {

    }

    public class AsSelfSongManager : ITransientDependency
    {

    }
}
