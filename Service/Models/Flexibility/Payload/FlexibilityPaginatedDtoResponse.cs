namespace Service.Models.Flexibility.Payload;

public class FlexibilityPaginatedDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<FlexibilityDto> Flexibilities { get; set; }
}