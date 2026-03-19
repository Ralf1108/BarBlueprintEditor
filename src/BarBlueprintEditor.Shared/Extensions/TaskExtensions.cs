namespace BarBlueprintEditor.Shared.Extensions;

/// <summary>
/// Taken from: https://github.com/dotnet/runtime/issues/97355
/// Task.WhenEach() is available starting at .Net 9.0
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Implements Task.WhenEach() for using with LINQ
    /// As we expected the streaming use case in LINQ we don't use native Task.WhenEach() as it materializes all tasks upfront which would enumerate the source completely
    /// </summary>
    /// <returns></returns>
    public static IAsyncEnumerable<T> WhenEach<T>(this IEnumerable<Task<T>> source)
    {
        return source.WhenEachWithDegreeOfParallelism();
    }

    /// <summary>
    /// Implements Task.WhenEach() for using with LINQ with control over the degree of parallelism
    /// As we expected the streaming use case in LINQ we don't use native Task.WhenEach() as it materializes all tasks upfront which would enumerate the source completely
    /// </summary>
    public static async IAsyncEnumerable<T> WhenEachWithDegreeOfParallelism<T>(
        this IEnumerable<Task<T>> source,
        int? degreeOfParallelism = null)
    {
        var concurrency = degreeOfParallelism ?? Environment.ProcessorCount * 2;
        var tasks = new HashSet<Task<T>>(concurrency);

        using var enumerator = source.GetEnumerator();

        // preload tasks
        for (var i = 0; i < concurrency; i++)
        {
            if (!AddTask())
                break;
        }

        var allTasksStarted = false;
        while (tasks.Any())
        {
            var task = await Task.WhenAny(tasks);
            tasks.Remove(task);
            yield return task.Result;

            if (!allTasksStarted && !AddTask())
                allTasksStarted = true;
        }

        yield break;

        bool AddTask()
        {
            if (!enumerator.MoveNext())
                return false;

            tasks.Add(enumerator.Current);
            return true;
        }
    }
}