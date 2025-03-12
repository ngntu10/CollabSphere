using System;
using System.Collections.Generic;

namespace CollabSphere.Common
{
    public class PaginationResponse<T>
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
        public List<T> Items { get; set; } = new();


        public PaginationResponse() { }


        public PaginationResponse(int page, int pages, int size, long total, List<T> items)
        {
            Page = page;
            Pages = pages;
            Size = size;
            Total = total;
            Items = items ?? new List<T>(); // Đảm bảo không bị null
        }
    }
}
