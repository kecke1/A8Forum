﻿using System.ComponentModel.DataAnnotations;

namespace A8Forum.ViewModels;

public class MemberViewModel
{
    public string? MemberId { get; set; }

    [Display(Name = "Forum Id")]
    public string? MemberName { get; set; }

    [Display(Name = "Display Name")]
    public string? MemberDisplayName { get; set; }

    public bool Guest { get; set; } = false;
}