using Baked.Domain.Configuration;

namespace Baked.Domain.Inspection;

public class AttributeCaptureType(DomainModelContext _context)
    : ICaptureType
{
    public string Id => _context.Identifier;

    public string BuildTitle(Type type) =>
        $"[[{type.Name.Replace("Attribute", string.Empty)}]]";

    public object? ConvertTarget<T>(T? target) =>
        target;
}