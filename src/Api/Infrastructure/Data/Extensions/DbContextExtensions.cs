using Microsoft.EntityFrameworkCore;

namespace Recipes.Api.Infrastructure.Data.Extensions;

// Adapted from https://stackoverflow.com/a/64343713 by Etienne Charland (CC BY-SA 4.0).
public static class DbContextExtensions
{
    /// <summary>
    /// Tracks inserts, updates and deletes by comparing a detached child collection
    /// with the latest collection loaded and tracked from the database.
    /// </summary>
    public static void TrackChildChanges<T>(
        this DbContext context,
        IList<T> children,
        IList<T> existingChildren,
        Func<T, T, bool> match)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(children);
        ArgumentNullException.ThrowIfNull(existingChildren);
        ArgumentNullException.ThrowIfNull(match);

        foreach (var existing in existingChildren.ToList())
        {
            if (!children.Any(child => match(child, existing)))
                existingChildren.Remove(existing);
        }

        var existingChildrenCopy = existingChildren.ToList();
        foreach (var child in children.ToList())
        {
            var existing = existingChildrenCopy.SingleOrDefault(item => match(item, child));

            if (existing is not null)
                context.Entry(existing).CurrentValues.SetValues(child);
            else
                existingChildren.Add(child);
        }
    }

    /// <summary>
    /// Copies scalar values from a detached model to its tracked database instance
    /// and persists the complete change set in a single SaveChanges operation.
    /// </summary>
    public static async Task SaveDetachedChangesAsync<T>(
        this DbContext context,
        T model,
        T existing,
        CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(existing);

        context.Entry(existing).CurrentValues.SetValues(model);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
