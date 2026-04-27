using Baked.Domain.Inspection;
using Baked.Testing;

namespace Baked.Test;

public class StubUxFeature(Stubber giveMe)
{
    readonly InspectTrace _trace = Inspect.TraceHere();

    public TSchema Configure<TSchema>(Func<TSchema> create) =>
        _trace.Capture(giveMe.AComponentContext(), create);
}