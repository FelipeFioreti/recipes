namespace Recipes.Api.Presentation.Models;

public record ErrorResponse
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
    public string? TraceId { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }
}
