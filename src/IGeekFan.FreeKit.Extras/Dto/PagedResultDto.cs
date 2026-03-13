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

    public PagedResultDto(IReadOnlyList<T> items) : this(items, items.Count)
    {
    }

    public PagedResultDto(IReadOnlyList<T> items, long count)
    {
        Items = items;
        Count = count;
    }

    public PagedResultDto(IReadOnlyList<T> items, BasePagingInfo page) : this(items, page.Count, page.PageNumber,
        page.PageSize)
    {
    }

    public PagedResultDto(IReadOnlyList<T> items, long count, int pageNumber, int pageSize)
    {
        Items = items;
        Count = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}