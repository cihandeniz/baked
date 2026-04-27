using Baked.Core;
using Baked.Runtime;
using Baked.Ui;
using Newtonsoft.Json;
using Spectre.Console;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Baked.Domain.Inspection;

internal class Capture<T>
{
    static JsonSerializerSettings SerializerSettings { get; } = new()
    {
        ContractResolver = new AttributeAwareCamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    static string FormatValue(object? value) =>
        value is string || value?.GetType().SkipNullable().IsValueType == true
            ? $"{value}"
            : JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);

    readonly Inspect _inspect;
    readonly StackTrace _stackTrace;
    readonly Func<T> _apply;
    readonly ICaptureType _captureType;
    readonly T? _givenTarget;
    readonly bool _initial;

    public Capture(Inspect inspect, StackTrace stackTrace, Func<T> create, ICaptureType captureType)
        : this(inspect, stackTrace, create, captureType, default, initial: true) { }

    public Capture(Inspect inspect, StackTrace stackTrace, Action update, ICaptureType captureType, T target)
        : this(inspect, stackTrace, () => { update(); return target; }, captureType, target, initial: false) { }

    Capture(Inspect inspect, StackTrace stackTrace, Func<T> apply, ICaptureType captureType, T? givenTarget, bool initial)
    {
        _inspect = inspect;
        _stackTrace = stackTrace;
        _apply = apply;
        _captureType = captureType;
        _givenTarget = givenTarget;
        _initial = initial;
    }

    string Property => _inspect.Expression.StripLambdaFromASingleMemberAccessExpression();

    public T Execute()
    {
        TryEvaluate(_givenTarget, out var previousValue, out var _);
        var target = _apply();
        if (!TryEvaluate(target, out var value, out var type)) { return target; }

        if (_initial)
        {
            Diagnostics.ReportInfo($"[lightskyblue3_1]{_captureType.BuildTitle(type)}[/] [gray]{_captureType.Id}[/]", group: _captureType.Id);
        }

        if (Equals(value, previousValue)) { return target; }

        var source = TryFindFeatureSource(out var featureSource)
            ? $"[magenta]{featureSource}[/]"
            : $"[magenta]<unknown>[/]{Environment.NewLine}[gray]{Markup.Escape($"{_stackTrace}")}[/]";
        Diagnostics.ReportInfo($"  [gray]{Property}:[/] {Markup.Escape(FormatValue(value))} ← {source}", group: _captureType.Id);

        return target;
    }

    bool TryEvaluate(T? target, out object? value, [NotNullWhen(true)] out Type? concreteTypeOfTarget)
    {
        value = null;
        concreteTypeOfTarget = null;

        object? targetObject = target is IComponentDescriptor descriptor
            ? descriptor.Schema
            : target;
        if (targetObject is null || !targetObject.GetType().IsAssignableTo(_inspect.TargetType)) { return false; }

        value = _inspect.Evaluate(targetObject);
        concreteTypeOfTarget = targetObject.GetType();

        return true;
    }

    bool TryFindFeatureSource([NotNullWhen(true)] out string? source)
    {
        source = null;

        var frames = _stackTrace.GetFrames();
        var featureFrame =
            frames.FirstOrDefault(f => f.GetMethod()?.ReflectedType?.DeclaringType?.Name.EndsWith("Feature") == true) ??
            frames.FirstOrDefault(f => f.GetMethod()?.DeclaringType?.Name.EndsWith("Feature") == true);
        if (featureFrame is null) { return false; }

        source =
            featureFrame.GetMethod()?.ReflectedType?.DeclaringType?.Name ??
            featureFrame.GetMethod()?.DeclaringType?.Name ??
            string.Empty;

        var fileName = featureFrame.GetFileName();
        if (fileName is null) { return true; }

        var title = source;
        var url = new Uri(fileName).AbsoluteUri;
        var lineNumber = featureFrame.GetFileLineNumber();
        if (lineNumber > 0)
        {
            title += $":{lineNumber}";
        }

        source = $"[link={url}]{title}[/]";

        return true;
    }
}