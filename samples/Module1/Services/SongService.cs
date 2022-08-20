using System.ComponentModel;
using System.Data;
using FreeSql;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.FreeSql;

namespace Module1.Services
{

    public class SongService : ISongService
    {
        private readonly IBaseRepository<Song, int> _useRepository;
        public SongService(IBaseRepository<Song, int> useRepository)
        {
            _useRepository = useRepository;
        }

        [Transactional(Propagation.Required)]
        public Song InsertSong(Song song)
        {
            _useRepository.Insert(song);
            return song;
        }

        public List<Song> GetSongs()
        {
            return _useRepository.Select.ToList();
        }

        public void DeleteSong(int id)
        {
            _useRepository.Delete(id);
        }
    }

    [Description("Song歌曲")]
    public class Song
    {
        /// <summary>
        /// 自增
        /// </summary>
        [Column(IsIdentity = true)]
        [Description("自增id")]
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
