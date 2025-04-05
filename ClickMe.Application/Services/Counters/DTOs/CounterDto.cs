namespace ClickMe.Application.Services.Counters.DTOs;

public sealed class CounterDto {
    public int CounterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}