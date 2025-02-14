using System.Collections;

namespace Domain.Models.Common
{
    public class PagedList<T> : IEnumerable<T> where T : class
    {
        public IEnumerable<T> Items { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
        public int CurrentPage { get; init; }
        public int EachPage { get; init; }

        public Metadata Metadata => new Metadata
        {
            TotalCount = TotalItems,
            TotalPages = TotalPages,
            CurrentPage = CurrentPage,
            PageSize = EachPage
        };

        public PagedList(IEnumerable<T> items)
        {
            Items = items;
            TotalItems = items.Count();
            TotalPages = 1;
            CurrentPage = 1;
            EachPage = items.Count();
        }

        public PagedList()
        {
            Items = new List<T>();
            TotalItems = 0;
            TotalPages = 1;
            CurrentPage = 1;
            EachPage = 0;
        }

        public PagedList(IEnumerable<T> items, int totalItems, int currentPage, int eachPage)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            EachPage = eachPage;
            TotalPages = (int)Math.Ceiling(totalItems / (double)eachPage);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
