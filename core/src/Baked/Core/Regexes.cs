using System.Reflection;
using System.Text.RegularExpressions;

namespace Baked.Core;

internal static partial class Regexes
{
    [GeneratedRegex(@"[\s\S]*?(?=.Application|$)")]
    public static partial Regex AssemblyNameBeforeApplicationSuffix { get; }

    extension(Assembly assembly)
    {
        public string GetNameBeforeApplicationSuffix() =>
            AssemblyNameBeforeApplicationSuffix
                .Match(assembly.FullName ?? string.Empty)
                .Value;
    }

    [GeneratedRegex(@"^\w+ => (Convert\()?\w+\.?")]
    public static partial Regex LambdaOfASingleMemberAccessExpression { get; }

    [GeneratedRegex(@"(,\s*\w+)?\)$")]
    public static partial Regex ExpressionEndNoise { get; }

    [GeneratedRegex(@"<>.*AnonymousType.*\(")]
    public static partial Regex AnonymousTypeNoise { get; }

    extension(string expression)
    {
        public string StripNoiseFromExpressionString()
        {
            var result =
                ExpressionEndNoise.Replace(
                    AnonymousTypeNoise.Replace(
                        LambdaOfASingleMemberAccessExpression.Replace(
                            expression,
                            string.Empty
                        ),
                        string.Empty
                    ),
                    string.Empty
                )
                .Trim();

            return result == string.Empty ? "<this>" : result;
        }
    }
}