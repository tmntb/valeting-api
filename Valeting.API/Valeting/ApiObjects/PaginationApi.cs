﻿namespace Valeting.ApiObjects
{
    public class PaginationApi
    {
        public int TotalItems { get; set; } = 1;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public PaginationLinksApi Links { get; set; } //https://code-maze.com/hateoas-aspnet-core-web-api/
    }

    public class PaginationLinksApi
    {
        public LinkApi Prev { get; set; }
        public LinkApi Next { get; set; }
        public LinkApi Self { get; set; }
    }
}

