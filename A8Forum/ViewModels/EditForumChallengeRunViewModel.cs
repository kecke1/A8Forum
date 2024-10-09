﻿using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class EditForumChallengeRunViewModel
{
    [Display(Name = "Id")]
    public string? ForumChallengeRunId { get; set; }

    public int Time { get; set; }

    [Display(Name = "Time")]
    public required string TimeString { get; set; }

    [Display(Name = "Insert Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime Idate { get; set; } = DateTime.Now;

    [Display(Name = "Post URL")]
    public required string Post { get; set; }

    public bool Deleted { get; set; } = false;
    public required string ForumChallengeId { get; set; }
    public string? MemberId { get; set; }
    public required string VehicleId { get; set; }
}