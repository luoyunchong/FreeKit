using System.Collections.Generic;
using FreeSql.Internal.Model;

namespace IGeekFan.FreeKit.Extras.Dto;

public class PagedResultDto<T> : BasePagingInfo where T : class
{
    public IReadOnlyList<T> Data { get; private set; }

    public PagedResultDto(IReadOnlyList<T> data)
    {
        Count = data.Count;
        Data = data;
    }

    public PagedResultDto(IReadOnlyList<T> data, BasePagingInfo page) : this(data)
    {
        PageNumber = page.PageNumber;
        PageSize = page.PageSize;
        Count = page.Count;
        Data = data;
    }
    
    public PagedResultDto(IReadOnlyList<T> data, long count) : this(data)
    {
        Count = count;
        Data = data;
    }

    public PagedResultDto(IReadOnlyList<T> data, long count, int pageNumber, int pageSize) : this(data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }
}

