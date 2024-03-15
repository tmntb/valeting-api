using System.Net;

namespace Valeting.Business;

public class ErrorDTO
{
    public Guid Id { get; set; }
    public int ErrorCode { get; set; }
    public string Detail { get; set; }
}