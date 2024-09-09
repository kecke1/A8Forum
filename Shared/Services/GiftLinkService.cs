using Microsoft.Azure.CosmosRepository;
using Shared.Dto;
using Shared.Mappers;
using Shared.Models;

namespace Shared.Services;

public class GiftLinkService(IMasterDataService masterDataService,
        IRepository<GiftLink> giftLinkRepository,
        IRepository<GiftLinkProvider> giftLinkProviderRepository)
    : IGiftLinkService
{
    public async Task AddGiftLinkAsync(GiftLinkDTO g)
    {
        await giftLinkRepository.CreateAsync(g.ToGiftLinkEntity());
    }

    public async Task AddGiftLinkProviderAsync(GiftLinkProviderDTO g)
    {
        await giftLinkProviderRepository.CreateAsync(g.ToGiftLinkProviderEntity());
    }

    public async Task DeleteGiftLinkAsync(string giftLinkId)
    {
        var g = await GetGiftLinkAsync(giftLinkId);
        g.Deleted = true;
        await giftLinkRepository.UpdateAsync(g.ToGiftLinkEntity());
    }

    public async Task DeleteGiftLinkProviderAsync(string giftLinkProviderId)
    {
        var g = await GetGiftLinkProviderAsync(giftLinkProviderId);
        g.Deleted = true;
        await giftLinkProviderRepository.UpdateAsync(g.ToGiftLinkProviderEntity());
    }

    public async Task<GiftLinkDTO> GetGiftLinkAsync(string giftLinkId)
    {
        var g = await giftLinkRepository.GetAsync(giftLinkId);
        var provider = await GetGiftLinkProviderAsync(g.GiftLinkProviderId);
        var member = await masterDataService.GetMemberAsync(g.MemberId);

        return g.ToDto(provider, member);
    }

    public async Task<GiftLinkProviderDTO> GetGiftLinkProviderAsync(string giftLinkProviderId)
    {
        var g = await giftLinkProviderRepository.GetAsync(giftLinkProviderId);
        return g.ToDto();
    }

    public async Task<IEnumerable<GiftLinkProviderDTO>> GetGiftLinkProvidersAsync()
    {
        var g = await giftLinkProviderRepository.GetAsync(x => true);
        return g.Select(x => x.ToDto());
    }

    public async Task<IEnumerable<GiftLinkDTO>> GetGiftLinksAsync()
    {
        var g = await giftLinkRepository.GetAsync(x => true);
        var providers = await GetGiftLinkProvidersAsync();
        var members = await masterDataService.GetMembersAsync();
        return g.Select(x => x.ToDto(providers.Single(y => y.Id == x.GiftLinkProviderId),
            members.Single(y => y.Id == x.MemberId)));
    }

    public async Task UpdateGiftLinkAsync(GiftLinkDTO g)
    {
        await giftLinkRepository.UpdateAsync(g.ToGiftLinkEntity());
    }

    public async Task UpdateGiftLinkProviderAsync(GiftLinkProviderDTO g)
    {
        await giftLinkProviderRepository.UpdateAsync(g.ToGiftLinkProviderEntity());
    }

    public string CreateGiftLinkList(IEnumerable<GiftLinkDTO> links)
    {
        var l = new List<string>();

        var grouped = links.GroupBy(x => x.Idate.DayOfYear).OrderBy(x => x.Key);
        foreach (var group in grouped)
        {
            l.Add($"[hr][b]{group.First().Idate:M}[/b]");
            foreach (var g in group)
            {
                l.Add(
                    $"[span style=\"white-space:nowrap;\"][a href=\"{g.Url}\"]{g.Url}[/a] - {g.GiftLinkProvider.Name} {(g.GiftLinkProvider.Url.StartsWith("http") ? $"([a href=\"{g.GiftLinkProvider.Url}\"]{g.GiftLinkProvider.Url}[/a])[/span]" : "")}");
                if (!string.IsNullOrEmpty(g.Notes))
                    l.Add($"[span style=\"white-space:nowrap;\"][i]({g.Notes})[/i][/span]");
                l.Add("");
            }
        }

        var msg = $@"
I will organize the {links.FirstOrDefault()?.Idate ?? DateTime.Now:MMMM} gift links in the same way as previous months.

Please share and post gift links here and I will add them to this post.

When you post links, it's helpful if you also include the source of the link so that we can keep track of which creators links has not been posted yet.

";
        if (links.Any())
        {
            msg +=
                $"Thanks to the persons who posted this months gift links:\n{string.Join(',', links.Select(x => $" {(x.Member.Guest ? "" : "@")}{x.Member.MemberName}{(x.Member.Guest ? " (Guest)" : "")}").Distinct())}\n";

            msg += $"[div style = \"overflow:auto;\"]\n{string.Join('\n', l)}[/div]";
        }

        return msg;
    }

    public IEnumerable<GiftLinkProviderWithDateDTO> GetActiveGiftLinkProviders(DateTime month,
        IEnumerable<GiftLinkProviderDTO> providers,
        IEnumerable<GiftLinkDTO> giftLinks)
    {
        var lastDayfMonth = new DateTime(month.Year, month.Month, 1).AddMonths(1).AddSeconds(-1);

        var q = providers
            .Where(x => x is { Hide: false, Deleted: false })
            .GroupJoin(giftLinks.Where(x => x.Month <= lastDayfMonth),
                y => y.Id, z => z.GiftLinkProvider.Id, (y, z) => new { Provider = y, Link = z })
            .SelectMany(x => x.Link.DefaultIfEmpty(),
                (x, y) => new { P = x.Provider, L = y ?? new GiftLinkDTO { Month = DateTime.MinValue } }).ToList();

        var q2 = q.GroupBy(x => new { ProviderId = x.P.Id, ProviderName = x.P.Name, x.P.Url })
            .Select(x => new GiftLinkProviderWithDateDTO
            {
                Id = x.Key.ProviderId,
                Name = x.Key.ProviderName,
                Url = x.Key.Url,
                Deleted = false,
                Hide = false,
                LatestGiftLinkDate = x.Max(y => y.L.Month)
            }).ToList();

        return q2;
    }

    public string CreateGiftLinkProviderList(IEnumerable<GiftLinkProviderWithDateDTO> giftLinkProviders, DateTime month)
    {
        var firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
        var firstDayOfLastMonth = new DateTime(month.Year, month.Month, 1).AddMonths(-1);
        // [h3] Youtube channels with gift links last month[/h3][ul][li]

        // [h3] Youtube channels without gift links last month[/h3][ul][li]
        var thisMonth = giftLinkProviders.Where(x => x.LatestGiftLinkDate >= firstDayOfMonth)
            .OrderBy(x => x.LatestGiftLinkDate).ToList();
        var prevMonths = giftLinkProviders
            .Where(x => x.LatestGiftLinkDate >= firstDayOfLastMonth && x.LatestGiftLinkDate < firstDayOfMonth)
            .OrderBy(x => x.LatestGiftLinkDate).ToList();
        var olderMonths = giftLinkProviders.Where(x => x.LatestGiftLinkDate < firstDayOfLastMonth)
            .OrderBy(x => x.LatestGiftLinkDate).ToList();

        var html =
            @"
Here's the YouTube channel links for the known content creators copied from the previous months thread.

I'll tick them off ✅ when the free gift links are added above.

";

        if (thisMonth.Any())
            html +=
                $"[ul style='list-style: none;']{string.Join("\n", thisMonth.Select(x => $"[li]\u2705{x.Name} - [a href='{x.Url}']{x.Url}[/a][/li]"))}[/ul]";

        if (prevMonths.Any())
            html +=
                $@"
[h3] Youtube channels with gift links last month[/h3][ul style='list-style: none;']{string.Join("\n", prevMonths.Select(x => $"[li]{x.Name} - [a href='{x.Url}']{x.Url}[/a][/li]"))}[/ul]";

        if (olderMonths.Any())
            html +=
                $@"
[h3] Youtube channels without gift links last month[/h3][ul style='list-style: none;']{string.Join("\n", olderMonths.Select(x => $"[li]{x.Name} - [a href='{x.Url}']{x.Url}[/a][/li]"))}[/ul]";

        return html;
    }
}