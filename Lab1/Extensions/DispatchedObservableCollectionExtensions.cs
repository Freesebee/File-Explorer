using Lab1.Models;
using Lab1.Resources;
using System.Diagnostics;

namespace Lab1.Extensions
{
    public static class DispatchedObservableCollectionExtensions
    {
        public static void Sort(this DispatchedObservableCollection<FileSystemInfoViewModel> collection, SortOptions options)
        {
            var query = options.Direction == SortOrder.Ascending
                ? options.SortBy switch
                {
                    SortBy.Alphabetic => collection.OrderBy(x => x.Name),
                    SortBy.Size => collection.OrderBy(x => x.Size),
                    SortBy.Extension => collection.OrderBy(x => x.Extension),
                    SortBy.Date => collection.OrderBy(x => x.LastWriteTime),
                    _ => throw new NotImplementedException(),
                }
                : options.SortBy switch
                {
                    SortBy.Alphabetic => collection.OrderByDescending(x => x.Name),
                    SortBy.Size => collection.OrderByDescending(x => x.Size),
                    SortBy.Extension => collection.OrderByDescending(x => x.Extension),
                    SortBy.Date => collection.OrderByDescending(x => x.LastWriteTime),
                    _ => throw new NotImplementedException(),
                };

            var sortedDirectoryQuery = query.Where(x => x is DirectoryInfoViewModel);
            var sortedFileNames = query.Where(x => x is FileInfoViewModel).Select(x => x.Name).ToList();

            var sortedDirectoryNames = sortedDirectoryQuery.Select(x => x.Name).ToList();
            var sortedItemNames = sortedDirectoryNames.Concat(sortedFileNames).ToList();

            for (int i = 0; i < sortedItemNames.Count(); i++)
            {
                var resultItemIndex = collection.First(x => x.Name == sortedItemNames[i]);
                collection.Move(collection.IndexOf(resultItemIndex), i);
            }

            var sortedDirectories = sortedDirectoryQuery.ToList();
            int subFoldersCount = sortedDirectories.Count;

            var tasks = new Task[subFoldersCount];

            Action<int> taskAction = (int index) =>
            {
                Debug.WriteLine($"{Strings.Sorting_directory}: {sortedDirectories[index].Caption}");
                (sortedDirectories[index] as DirectoryInfoViewModel)?.Sort(options);
            };

            Debug.WriteLine($"ThreadID: {Thread.CurrentThread.ManagedThreadId}");
            //Zad 4.1 ODP: Zawsze jeden wątek (wszystkie mają ID głównego), dla TaskCreationOptions.None
            //Zad 4.2 ODP: Tyle nowych wątków, ile jest zadań, dla TaskCreationOptions.LongRunning

            for (int i = 0; i < tasks.Length; i++)
            {
                int index = i;
                tasks[i] = Task.Factory.StartNew(() => taskAction(index), TaskCreationOptions.LongRunning);
            }

            Task.WaitAll(tasks);
        }
    }
}
