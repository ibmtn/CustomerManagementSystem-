using System.Collections.Generic;

namespace MusteriYonetim.Models.Dtos
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public long? NextCursor { get; set; } // Bir sonraki sayfa için kullanılacak son ID (lastId)
        public bool HasNextPage { get; set; }
        public int PageSize { get; set; }
    }
}
