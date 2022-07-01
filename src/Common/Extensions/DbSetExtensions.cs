using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> GetById<T>(this DbSet<T> dbSet, Guid id)
        where T : class
    {
        var entity = await dbSet.FindAsync(id);

        if (entity is null)
            throw new EntityNotFoundException();

        return entity;
    }

    public static async Task<bool> CheckIfExists<T>(this DbSet<T> dbSet, Guid id)
        where T : class
    {
        var entity = await dbSet.FindAsync(id);

        return entity is not null;
    }
}