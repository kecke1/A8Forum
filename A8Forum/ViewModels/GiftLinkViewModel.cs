using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class GiftLinkViewModel
{
    public string? GiftLinkId { get; set; }
    public string? Url { get; set; }
    public GiftLinkProvider GiftLinkProvider { get; set; } = new GiftLinkProvider();

    [Display(Name = "Submited By")]
    public MemberViewModel SubmitedBy { get; set; } = new MemberViewModel();

    public bool Deleted { get; set; } = false;

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
    public DateTime Idate { get; set; } = DateTime.Now;

    [Display(Name = "Month")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
    public DateTime Month { get; set; } = DateTime.Now;

    public string? Notes { get; set; }

    [Display(Name = "Ignore duplicate validation")]
    public bool IgnoreDuplicateValidation { get; set; } = false;
}