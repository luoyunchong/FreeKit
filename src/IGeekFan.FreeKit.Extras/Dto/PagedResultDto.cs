// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql.Internal.Model;

namespace IGeekFan.FreeKit.Extras.Dto;

public class PagedResultDto<T> : BasePagingInfo where T : class
{
    public IReadOnlyList<T> Items { get; set; }

    public PagedResultDto()
    {
    }

    public PagedResultDto(IReadOnlyList<T> data) : this(data, data.Count)
    {
    }

    public PagedResultDto(IReadOnlyList<T> data, long count)
    {
        Count = count;
        Items = data;
    }

    public PagedResultDto(IReadOnlyList<T> data, BasePagingInfo page) : this(data, page.Count, page.PageNumber,
        page.PageSize)
    {
    }

    public PagedResultDto(IReadOnlyList<T> data, long count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Count = count;
        Items = data;
    }
}