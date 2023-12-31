﻿using Microsoft.EntityFrameworkCore;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IEntity<TIDType> where TIDType : notnull
    {
        TIDType Id { get; set; }
        string GetTableName(DbContext? dbContext = null);
        object? GetValue(string propName);
        IEnumerable<string> GetColumnNames();
    }
}