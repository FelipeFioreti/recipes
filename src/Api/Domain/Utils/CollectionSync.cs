namespace Recipes.Api.Domain.Utils;

public class CollectionSync
{
    public static void Sync<TEntity, TRequest, TKey>(
        ICollection<TEntity> entities,
        IEnumerable<TRequest> requests,
        Func<TEntity, TKey> entityKey,
        Func<TRequest, TKey> requestKey,
        Func<TRequest, bool> isNew,
        Func<TRequest, TEntity> create,
        Action<TEntity, TRequest> update)
        where TEntity : class
        where TKey : notnull
    {
        var requestList = requests.ToList();
        var dictEntities = entities.ToDictionary(entityKey);

        foreach (var request in requestList)
        {
            if (isNew(request))
            {
                entities.Add(create(request));
                continue;
            }

            var key = requestKey(request);

            if (!dictEntities.TryGetValue(key, out var entity))
                throw new KeyNotFoundException($"Entidade '{key}' não encontrada.");

            update(entity, request);
        }
    }
}