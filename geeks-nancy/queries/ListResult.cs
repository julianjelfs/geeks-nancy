using System.Collections.Generic;

namespace geeks_nancy.queries
{
    public class ListResult<T>
    {
        public IEnumerable<T> List { get; set; }
        public int TotalPages { get; set; }
    }
}