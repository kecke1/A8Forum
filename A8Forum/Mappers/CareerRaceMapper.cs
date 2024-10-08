﻿using A8Forum.ViewModels;
using Shared.Dto;

namespace A8Forum.Mappers;

internal static class CareerRaceMapper
{
    public static CareerRaceDTO ToDto(this CareerRaceViewModel r)
    {
        return new CareerRaceDTO
        {
            Id = r.CareerRaceId,
            CareerRaceType = r.CareerRaceType,
            Class = r.Class,
            Column = r.Column,
            Limitations = r.Limitations,
            LimitationsDescription = r.LimitationsDescription,
            Row = r.Row,
            Season = r.Season.ToDto(),
            Track = r.Track.ToDto()
        };
    }

    public static CareerRaceViewModel ToCareerRaceViewModel(this CareerRaceDTO model)
    {
        return new CareerRaceViewModel
        {
            CareerRaceId = model.Id,
            CareerRaceType = model.CareerRaceType,
            Class = model.Class,
            Column = model.Column,
            Limitations = model.Limitations,
            LimitationsDescription = model.LimitationsDescription,
            Row = model.Row,
            Season = model.Season.ToSeasonViewModel(),
            Track = model.Track.ToTrackViewModel()
        };
    }
}