using System.Data;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using Module1.Domain;

namespace Module1.Services
{

    public class SongService : ISongService
    {
        private readonly IBaseRepository<Song, int> _songRepository;
        public SongService(IBaseRepository<Song, int> songRepository)
        {
            _songRepository = songRepository;
        }

        [Transactional(Propagation.Required)]
        public Song InsertSong(Song song)
        {
            _songRepository.Insert(song);
            return song;
        }

        public List<Song> GetSongs()
        {
            return _songRepository.Select.ToList();
        }

        public void DeleteSong(int id)
        {
            _songRepository.Delete(id);
        }
    }

    public class AsSelfSongService
    {
        private readonly IBaseRepository<Song, int> _songRepository;
        public AsSelfSongService(IBaseRepository<Song, int> songRepository)
        {
            _songRepository = songRepository;
        }
        [Transactional(Propagation.Required)]
        public virtual Song InsertSong(Song song)
        {
            _songRepository.Insert(song);
            return song;
        }

    }
}
