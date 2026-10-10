namespace Baked.Ui;

public record MissingComponent : IComponentSchema
{
    public List<string> Path { get; init; } = [];
    public DomainSource? Source { get; set; }
    public string? Component { get; set; }

    public record DomainSource
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Path { get; init; } = [];
    }
}