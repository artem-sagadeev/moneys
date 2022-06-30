using Common.Interfaces;

namespace Common.Extensions;

public static class EntityExtensions
{
    public static List<Guid> Ids(this IEnumerable<IEntity> entities)
    {
        return entities
            .Select(entity => entity.Id)
            .ToList();
    }
}