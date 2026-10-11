// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql.Internal.Model;

namespace IGeekFan.FreeKit.Extras.Dto;

public class PagedResultDto<T> : BasePagingInfo where T : class
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    public PagedResultDto()
    {
    }

    public PagedResultDto(IReadOnlyList<T> items) : this(RequireItems(items), items.Count)
    {
    }

    public PagedResultDto(IReadOnlyList<T> items, long count)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        Count = count;
    }

    public PagedResultDto(IReadOnlyList<T> items, BasePagingInfo page) : this(RequireItems(items), RequirePage(page).Count, RequirePage(page).PageNumber,
        RequirePage(page).PageSize)
    {
    }

    public PagedResultDto(IReadOnlyList<T> items, long count, int pageNumber, int pageSize)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        Count = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    private static IReadOnlyList<T> RequireItems(IReadOnlyList<T> items) =>
        items ?? throw new ArgumentNullException(nameof(items));

    private static BasePagingInfo RequirePage(BasePagingInfo page) =>
        page ?? throw new ArgumentNullException(nameof(page));
}
