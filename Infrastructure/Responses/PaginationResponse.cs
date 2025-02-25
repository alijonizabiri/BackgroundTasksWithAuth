using System.Net;
using Domain.Filters;

namespace Infrastructure.Responses;

public class PaginationResponse<T> : Response<T>
{
    public int PageNumber { get; set; } // 2
    public int PageSize { get; set; } // 12
    public int TotalPages { get; set; } // 30
    public int TotalRecords { get; set; } // 350
    
    public PaginationResponse(T data, int totalRecords, int pageNumber, int pageSize) : base(data)
    {
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public PaginationResponse(HttpStatusCode code, string error) : base(code, error)
    {
    }
    
    public PaginationResponse()
    {

    }
}