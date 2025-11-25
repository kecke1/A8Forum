using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class SprintRunsViewModel
{
    [Display(Name = "Show all runs")]
    public bool ShowAllRuns { get; set; } = false;

    public string Names { get; set; } = "";
    public string Tracks { get; set; } = "";
    public string TodaysTrack { get; set; }
}
