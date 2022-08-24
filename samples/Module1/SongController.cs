using IGeekFan.FreeKit.Extras.FreeSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Module1.Domain;
using Module1.Services;

namespace Module1;


[Route("[module]/[controller]")]
public class SongController : ControllerBase
{
    private readonly ILogger<SongController> _logger;
    private readonly ISongService _songService;
    private readonly SongService _songService2;
    private readonly ITransientSongManager _songManager;
    private readonly IScopedSongManager _scopedSongManager;
    private readonly ISingletonSongManager _singletonSongManager;
    private readonly AsSelfSongManager _asSelfSongManager;
    private readonly AsSelfSongService _asSelfSongService;
    private readonly TransientSongManager _transientSongManager;

    public SongController(ILogger<SongController> logger, ISongService testService, ITransientSongManager songManager, IScopedSongManager scopedSongManager, ISingletonSongManager singletonSongManager, AsSelfSongManager asSelfSongManager, AsSelfSongService asSelfSongService, TransientSongManager transientSongManager, SongService songService2)
    {
        _logger = logger;
        _songService = testService;
        _songManager = songManager;
        _scopedSongManager = scopedSongManager;
        _singletonSongManager = singletonSongManager;
        _asSelfSongManager = asSelfSongManager;
        _asSelfSongService = asSelfSongService;
        _transientSongManager = transientSongManager;
        _songService2 = songService2;
    }

    [Transactional]
    [HttpPost]
    public Song InsertSong([FromBody] Song song)
    {
        _logger.LogInformation("InsertSong");
        _songService.InsertSong(song);

        if (song.Title == "e")
        {
            throw new Exception("出现错误，会回滚吗");
        }

        return song;
    }
    [Transactional]
    [HttpPost("asself")]
    public Song InsertAsSelfSong([FromBody] Song song)
    {
        _logger.LogInformation("InsertSong");
        _asSelfSongService.InsertSong(song);

        if (song.Title == "e")
        {
            throw new Exception("出现错误，会回滚吗");
        }

        return song;
    }

    [HttpGet]
    public List<Song> GetSongs()
    {
        return _songService.GetSongs();
    }

    [HttpDelete]
    public void Delete(int id)
    {
        _songService.DeleteSong(id);
    }

    [HttpGet("gethashcode")]
    public dynamic GetAllHashCode()
    {
        return new
        {
            transient = _songManager.GetHashCode(),
            transientSongManager= _transientSongManager.GetHashCode(),
            scope = _scopedSongManager.GetHashCode(),
            singleton = _singletonSongManager.GetHashCode(),
            asSelf = _asSelfSongManager.GetHashCode()
        };
    }

    /// <summary>
    /// InterModule
    /// </summary>
    /// <returns></returns>
    [HttpGet("InterModule")]
    public ActionResult<string> InterModule()
    {
        return $"{0} in TestController in Module 1 InterModule";
    }
}