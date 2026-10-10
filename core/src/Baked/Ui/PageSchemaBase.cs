namespace Baked.Ui;

public record PageSchemaBase : IPageSchema
{
    string _path = string.Empty;

    public string Path
    {
        get => _path;
        set => _path = value.Trim('/');
    }

    public string? Layout { get; set; }
}