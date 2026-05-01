namespace Baked.Ui;

public record FormPage(string Path, PageTitle Title, Button Submit)
    : PageSchemaBase(Path)
{
    public PageTitle Title { get; set; } = Title;
    public Button Submit { get; set; } = Submit;
    public List<Input> Inputs { get; init; } = [];
    public List<Section> Sections { get; init; } = [];
    public Dictionary<string, HashSet<string>> Groups { get; init; } = [];
    public HashSet<string> Wide { get; init; } = [];
    public bool? SingleColumn { get; set; }

    public record Section(string Key, string Label)
        : IOrderableSchema
    {
        public string Key { get; set; } = Key;
        public string Label { get; set; } = Label;
        public HashSet<string> Inputs { get; init; } = [];
    }
}