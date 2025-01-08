using System.ComponentModel;

using FreeSql.DataAnnotations;

using IGeekFan.FreeKit.Extras.Domain;

namespace Module1.Domain;

[Description("Song歌曲")]
public class Song : IDomainEventBase
{
    /// <summary>
    /// 自增
    /// </summary>
    [Column(IsIdentity = true)]
    [Description("自增id")]
    public int Id { get; set; }
    public string Title { get; set; }
    public List<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();

    public object[] GetKeys()
    {
        return new object[] { Id };
    }

    public void UpdateTitle(string title)
    {
        Title = title;
        DomainEvents.Add(new SongTitleChangedEvent(this));
    }
}

public class SongTitleChangedEvent : IDomainEvent
{
    public Song Song { get; }

    public SongTitleChangedEvent(Song song)
    {
        Song = song;
    }
}

public class SongTitleChangedEventHandler : IDomainEventHandler<SongTitleChangedEvent>
{
    public Task Handle(SongTitleChangedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Song Title Changed: {notification.Song.Title}");
        return Task.CompletedTask;
    }
}
