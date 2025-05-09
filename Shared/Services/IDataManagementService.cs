﻿using Shared.Dto;

namespace Shared.Services;

public interface IDataManagementService
{
    public Task<ExportDataDto> ExportData();
    public Task ImportData(ExportDataDto data);

}