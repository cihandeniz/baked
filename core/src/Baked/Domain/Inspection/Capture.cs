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
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    static string FormatValue(object? value) =>
        value is string || value?.GetType().SkipNullable().IsValueType == true
            ? $"{value}"
            : JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);

    readonly Inspection _inspection;
    readonly StackTrace _stackTrace;
    readonly Func<T> _apply;
    readonly ICaptureType _captureType;
    readonly T? _givenTarget;
    readonly bool _initial;

    public Capture(Inspection inspection, StackTrace stackTrace, Func<T> create, ICaptureType captureType)
        : this(inspection, stackTrace, create, captureType, default, initial: true) { }

    public Capture(Inspection inspection, StackTrace stackTrace, Action update, ICaptureType captureType, T target)
        : this(inspection, stackTrace, () => { update(); return target; }, captureType, target, initial: false) { }

    Capture(Inspection inspection, StackTrace stackTrace, Func<T> apply, ICaptureType captureType, T? givenTarget, bool initial)
    {
        _inspection = inspection;
        _stackTrace = stackTrace;
        _apply = apply;
        _captureType = captureType;
        _givenTarget = givenTarget;
        _initial = initial;
    }

    string Property => _inspection.Expression.StripLambdaFromASingleMemberAccessExpression();

    public T Execute()
    {
        TryEvaluate(_givenTarget, out var previousValue, out var _);
        var target = _apply();
        if (!TryEvaluate(target, out var value, out var type)) { return target; }

        if (_initial)
        {
            Diagnostics.Current.ReportInfo($"[lightskyblue3_1]{_captureType.BuildTitle(type)}[/] [gray]{_captureType.Id}[/]", group: _captureType.Id);
        }

        if (Equals(value, previousValue)) { return target; }

        var source = TryFindFeatureSource(out var featureSource)
            ? $"[magenta]{featureSource}[/]"
            : $"[magenta]<unknown>[/]{Environment.NewLine}[gray]{Markup.Escape($"{_stackTrace}")}[/]";
        Diagnostics.Current.ReportInfo($"  [gray]{Property}:[/] {Markup.Escape(FormatValue(value))} ← {source}", group: _captureType.Id);

        return target;
    }

    bool TryEvaluate(T? target, out object? value, [NotNullWhen(true)] out Type? concreteTypeOfTarget)
    {
        value = null;
        concreteTypeOfTarget = null;

        object? targetObject = target is IComponentDescriptor descriptor
            ? descriptor.Schema
            : target;
        if (targetObject is null || !targetObject.GetType().IsAssignableTo(_inspection.TargetType)) { return false; }

        value = _inspection.Evaluate(targetObject);
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