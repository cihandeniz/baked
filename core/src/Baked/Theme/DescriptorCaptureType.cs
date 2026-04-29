using Baked.Domain.Inspection;
using Baked.Ui;

namespace Baked.Theme;

public class DescriptorCaptureType(ComponentContext _context)
    : ICaptureType
{
    public string Id => $"{_context.Path}";

    public string BuildTitle(Type type) =>
        $"<{type.GetName(includeDeclaringTypes: true)}>";

    public object? ConvertTarget<T>(T? target) =>
        target is IComponentDescriptor descriptor
            ? descriptor.Schema
            : target;
}