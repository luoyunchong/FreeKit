using System.ComponentModel;
using FreeSql.DataAnnotations;

namespace Module1.Domain;

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