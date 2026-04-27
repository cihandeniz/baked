using Baked.Domain.Configuration;
using Baked.Domain.Inspection;

namespace Baked.Test.Domain;

public class StubFeature(DomainModelContext c)
{
    readonly InspectTrace _trace = Inspect.TraceHere();

    public TSchema Configure<TSchema>(Func<TSchema> create) =>
        _trace.Capture(c, create);
}