using Baked.Domain.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Baked.Domain.Inspection;

public class InspectTrace(StackTrace stackTrace)
{
    static bool ShouldCapture(DomainModelContext context, [NotNullWhen(true)] out Inspect? inspect)
    {
        inspect = Inspect.Current;

        return inspect is not null && inspect.Filter(context);
    }

    public StackTrace StackTrace { get; } = stackTrace;

    public T Capture<T>(DomainModelContext context, Func<T> create)
    {
        if (!ShouldCapture(context, out var inspect))
        {
            return create();
        }

        return
            new Capture<T>(inspect, StackTrace, create, new AttributeCaptureType(context))
            .Execute();
    }

    public T Capture<T>(DomainModelContext context, T target, Action update)
    {
        if (!ShouldCapture(context, out var inspect))
        {
            update();

            return target;
        }

        return
            new Capture<T>(inspect, StackTrace, update, new AttributeCaptureType(context), target)
            .Execute();
    }
}