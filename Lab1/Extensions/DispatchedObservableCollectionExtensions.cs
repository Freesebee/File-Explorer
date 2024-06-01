using Lab1.Models;

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

            var sortedDirectories = query.Where(x => x is DirectoryInfoViewModel).Select(x => x.Name).ToList();
            var sortedFiles = query.Where(x => x is FileInfoViewModel).Select(x => x.Name).ToList();

            var sortedItemNames = sortedDirectories.Concat(sortedFiles).ToList();

            for (int i = 0; i < sortedItemNames.Count(); i++)
            {
                var resultItemIndex = collection.First(x => x.Name == sortedItemNames[i]);
                //collection.Move(collection.IndexOf(sortedItems[i]), i); //todo remove
                collection.Move(collection.IndexOf(resultItemIndex), i);
            }

            foreach (var itemViewModel in collection)
            {
                (itemViewModel as DirectoryInfoViewModel)?.Sort(options);
            }
        }
    }
}
