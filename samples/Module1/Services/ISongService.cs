
using IGeekFan.FreeKit.Extras.Dependency;
using Module1.Domain;

namespace Module1.Services
{
    public interface ISongService : ITransientDependency
    {
        Song InsertSong(Song song);
        List<Song> GetSongs();
        void DeleteSong(int id);
    }
}