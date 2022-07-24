using Common.Exceptions;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> GetById<T>(this DbSet<T> dbSet, Guid id)
        where T : class, IEntity
    {
        var entity = await dbSet.FindAsync(id);

        if (entity is null)
            throw new EntityNotFoundException();

        return entity;
    }
    
    public static async Task<List<T>> GetByUserId<T>(this DbSet<T> dbSet, string userId)
        where T : class, IUserBelonging
    {
        var entities = await dbSet.Where(entity => entity.UserId == userId).ToListAsync();

        return entities;
    }

    public static async Task CheckIfExists<T>(this DbSet<T> dbSet, Guid id)
        where T : class, IEntity
    {
        var entity = await dbSet.FindAsync(id);

        if (entity is null)
            throw new EntityNotFoundException();
    }
}