using Baked.Domain.Inspection;

namespace Baked.Theme;

public class DescriptorCaptureType(ComponentContext _context)
    : ICaptureType
{
    public string Id => $"{_context.Path}";

    public string BuildTitle(Type type) =>
        $"<{type.GetName(includeDeclaringTypes: true)}>";
}