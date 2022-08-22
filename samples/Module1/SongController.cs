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
    private readonly ITransientSongManager _songManager;
    private readonly IScopedSongManager _scopedSongManager;
    private readonly ISingletonSongManager _singletonSongManager;

    public SongController(ILogger<SongController> logger, ISongService testService, ITransientSongManager songManager, IScopedSongManager scopedSongManager, ISingletonSongManager singletonSongManager)
    {
        this._logger = logger;
        this._songService = testService;
        _songManager = songManager;
        _scopedSongManager = scopedSongManager;
        _singletonSongManager = singletonSongManager;
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
    public dynamic GetHashCode()
    {
        return new
        {
            transient = _songManager.GetHashCode(),
            scope = _scopedSongManager.GetHashCode(),
            singleton = _singletonSongManager.GetHashCode(),
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