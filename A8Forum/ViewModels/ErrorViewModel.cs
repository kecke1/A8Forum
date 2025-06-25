namespace A8Forum.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? ErrorMessage { get; set; }
    public string? ErrorInnerException { get; set; }
    public string? StackTrace { get; set; }
}