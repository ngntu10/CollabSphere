using System;

namespace CollabSphere.Modules.Follow.Models
{
    public class SearchUserRequest
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
