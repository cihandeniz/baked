namespace Baked.Business;

[AttributeUsage(AttributeTargets.All)]
public class GroupAttribute : Attribute
{
    public string Name { get; set; } = "Default";
}