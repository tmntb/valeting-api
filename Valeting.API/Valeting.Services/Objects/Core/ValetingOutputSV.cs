namespace Valeting.Services.Objects.Core;

public class ValetingOutputSV
{
    public bool HasError { get { return Error != null && Error.ErrorCode != 0 && !string.IsNullOrEmpty(Error.Message); } }
    public ValetingErrorSV Error { get; set; }
}

public class ValetingErrorSV
{
    public int ErrorCode { get; set; }
    public string Message { get; set; }
}
