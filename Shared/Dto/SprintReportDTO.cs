﻿using System.ComponentModel.DataAnnotations;

namespace Shared.Dto;

public class SprintReportDTO
{
    [Display(Name = "Best lap times BB Code")]
    public required string LeaderBoardByMember { get; set; }

    [Display(Name = "Leaderboards BB Code")]
    public required string LeaderBoardByTrack { get; set; }

    [Display(Name = "Best lap times")]
    public string? LeaderBoardByMemberHtml { get; set; }

    [Display(Name = "Leaderboards")]
    public string? LeaderBoardByTrackHtml { get; set; }

    [Display(Name = "Total Leaderboard BB Code")]
    public required string TotalLeaderBoard { get; set; }

    [Display(Name = "Total Leaderboard no VIP BB Code")]
    public required string TotalLeaderBoardNoVip { get; set; }

    [Display(Name = "Total Leaderboard no VIP")]
    public string TotalLeaderBoardNoVipHtml { get; set; }
    [Display(Name = "Total Leaderboard")]
    public string? TotalLeaderBoardHtml { get; set; }

    [Display(Name = "Leaderboard Json")]
    public string? LeaderBoardJson { get; set; }
}