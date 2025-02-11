using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObject.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, int totalPage)
        {
            this.PageNumber = pageNumber;
            this.TotalPage = totalPage;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
