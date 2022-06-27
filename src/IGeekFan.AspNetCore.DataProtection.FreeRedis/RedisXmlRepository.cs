// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FreeRedis;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace IGeekFan.AspNetCore.DataProtection.FreeRedis;

/// <summary>
/// An XML repository backed by a Redis list entry.
/// </summary>
public class RedisXmlRepository : IXmlRepository
{
    private readonly Func<IRedisClient> _databaseFactory;
    private readonly string _key;

    /// <summary>
    /// Creates a <see cref="RedisXmlRepository"/> with keys stored at the given directory.
    /// </summary>
    /// <param name="databaseFactory">The delegate used to create <see cref="IRedisClient"/> instances.</param>
    /// <param name="key">The key used to store key list.</param>
    public RedisXmlRepository(Func<IRedisClient> databaseFactory, string key)
    {
        _databaseFactory = databaseFactory;
        _key = key;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        return GetAllElementsCore().ToList().AsReadOnly();
    }

    private IEnumerable<XElement> GetAllElementsCore()
    {
        // Note: Inability to read any value is considered a fatal error (since the file may contain
        // revocation information), and we'll fail the entire operation rather than return a partial
        // set of elements. If a value contains well-formed XML but its contents are meaningless, we
        // won't fail that operation here. The caller is responsible for failing as appropriate given
        // that scenario.
        var database = _databaseFactory();
        foreach (var value in database.LRange(_key, 0, -1))
        {
            yield return XElement.Parse(value);
        }
    }

    /// <inheritdoc />
    public void StoreElement(XElement element, string friendlyName)
    {
        var database = _databaseFactory();
        database.RPush(_key, element.ToString(SaveOptions.DisableFormatting));
    }
}
