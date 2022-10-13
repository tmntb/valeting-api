using System;
namespace Valeting.ApiObjects
{
    public abstract class QueryStringParametersApi
    {
        //implementação da paginação
        //https://code-maze.com/paging-aspnet-core-webapi/

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

