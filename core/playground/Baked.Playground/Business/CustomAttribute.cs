namespace Baked.Playground.Business;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter)]
public class CustomAttribute : Attribute
{
    public string Value { get; set; } = string.Empty;
    public string? NullableValue { get; set; }
}