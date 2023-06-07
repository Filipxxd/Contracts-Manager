namespace InsuranceApp.Models.ViewModels.Shared
{
    /// <summary>
    /// Represents a paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Current page index.
        /// </summary>
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        /// <summary>
        /// Initializes a new instance with the specified items, total count, page index, and page size.
        /// </summary>
        /// <param name="items">The items in the list.</param>
        /// <param name="count">The total number of items in the source.</param>
        /// <param name="pageIndex">The current page index.</param>
        /// <param name="pageSize">The number of items to be shown per page.</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = count < pageSize ? 1 : (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Creates a <see cref="PaginatedList{T}"/> based on the source list, page index, and page size.
        /// </summary>
        /// <param name="source">The source list of items</param>
        /// <param name="pageIndex">The current page index</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>A new instance of the <see cref="PaginatedList{T}"/> class.</returns>
        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
        {
            int actualSize = source.Count < pageSize ? source.Count : pageSize;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(actualSize).ToList();

            return new PaginatedList<T>(items, source.Count, pageIndex, pageSize);
        }
    }
}
