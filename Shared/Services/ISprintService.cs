﻿using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;
public interface ISprintService
{
    public Task<IEnumerable<SprintRunDTO>> GetSprintRunsAsync();

    public Task<SprintRunDTO> GetSprintRunAsync(string sprintRunId);

    public Task UpdateSprintRunAsync(EditSprintRunDTO r);

    public Task AddSprintRunAsync(EditSprintRunDTO r);

    public Task DeleteSprintRunAsync(string sprintRunId);

    public Task ImportSprintRunsAsync(SprintImportDTO runs);

    public Task<IOrderedEnumerable<SprintLeaderboardRowDto>> GetSprintLeaderboardRowsAsync();

    public Task<string> GetSprintLeaderboardByMemberAsync(IOrderedEnumerable<SprintLeaderboardRowDto> runs);

    public Task<string> GetSprintLeaderboardByTrackAsync(IOrderedEnumerable<SprintLeaderboardRowDto> runs);

    public Task<string> GetSprintTotalLeaderboardTableAsync(IEnumerable<SprintLeaderboardRowDto> runs);

    public Task<SprintReportDTO> GetSprintReportAsync();

    public Task<SprintRunDTO> GetSprintRunFromTemplateAsync(string template, string postUrl);

}
