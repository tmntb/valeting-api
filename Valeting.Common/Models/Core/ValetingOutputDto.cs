namespace Valeting.Common.Models.Core;

public class ValetingOutputDto
{
    public bool HasError { get { return Error != null && Error.ErrorCode != 0 && !string.IsNullOrEmpty(Error.Message); } }
    public ValetingErrorDto Error { get; set; }
}

public class ValetingErrorDto
{
    public int ErrorCode { get; set; }
    public string Message { get; set; }
}
