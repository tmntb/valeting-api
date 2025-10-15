namespace Service.Models.Link.Payload;

public class GeneratePaginatedLinksDtoResponse
{
    public string Self { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
}
