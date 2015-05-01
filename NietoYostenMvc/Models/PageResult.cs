using System.Collections.Generic;

namespace NietoYostenMvc.Models
{
    public class PageResult
    {
        private PageResult()
        {
            // do nothing
        }

        public static PageResult GetInstance(IEnumerable<dynamic> items, int totalPages)
        {
            return new PageResult {Items = items, TotalPages = totalPages};
        }

        public IEnumerable<dynamic> Items { get; private set; }

        public int TotalPages { get; private set; }
    }
}