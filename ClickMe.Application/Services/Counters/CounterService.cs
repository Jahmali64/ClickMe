using ClickMe.Application.Services.Counters.DTOs;
using ClickMe.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClickMe.Application.Services.Counters;

public interface ICounterService {
    Task<List<CounterDto>> GetAllAsync();
}

public sealed class CounterService : ICounterService {
    private readonly IDbContextFactory<ClickMeDbContext> _clickMeDbContextFactory;
    private readonly CancellationToken _cancellationToken;
    
    public CounterService(IDbContextFactory<ClickMeDbContext> clickMeDbContextFactory, CancellationToken cancellationToken) {
        _clickMeDbContextFactory = clickMeDbContextFactory;
        _cancellationToken = cancellationToken;
    }

    public async Task<List<CounterDto>> GetAllAsync() {
        await using ClickMeDbContext db = await _clickMeDbContextFactory.CreateDbContextAsync(_cancellationToken);
        
        return await db.Counters.Where(counter => counter.Trash == 0).Select(counter => new CounterDto {
            CounterId = counter.CounterId,
            Name = counter.Name ?? string.Empty,
            Count = counter.Count,
        }).ToListAsync(_cancellationToken);
    }
}
