using Baked.Ui;

namespace Baked.Theme.Admin;

public record SelectButton : IComponentSchema
{
    public bool AllowEmpty { get; set; }
    public string? OptionLabel { get; set; }
    public string? OptionValue { get; set; }
}