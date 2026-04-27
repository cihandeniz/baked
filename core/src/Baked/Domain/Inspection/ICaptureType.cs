namespace Baked.Domain.Inspection;

internal interface ICaptureType
{
    string Id { get; }
    string BuildTitle(Type type);
}