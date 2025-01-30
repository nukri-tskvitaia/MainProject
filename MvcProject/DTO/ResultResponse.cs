namespace MvcProject.DTO;

public class ResultResponse
{
    public string Status { get; set; } = default!;
    public string? ErrorMessage { get; set; }
}
