﻿using Microsoft.Azure.CosmosRepository;

namespace Shared.Models;

public class ForumChallengeRun : Item
{
    public int Time { get; set; }
    public DateTime Idate { get; set; } = DateTime.Now;
    public string? Post { get; set; } = "";
    public bool Deleted { get; set; } = false;
    public required string ForumChallengeId { get; set; }
    public required string MemberId { get; set; }
    public string? VehicleId { get; set; }
}