using System;

namespace MusteriYonetim.ViewModels
{
    public class PaginationBaseViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalItems { get; set; }
        public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);
        public long? NextCursor { get; set; }
        public bool HasNextPage { get; set; }
        public long? CurrentCursor { get; set; } // Cursor of the current page's first item, if needed
    }
}
