﻿using Baked.CodingStyle;
using Baked.CodingStyle.CommandPattern;

namespace Baked;

public static class CommandPatternCodingStyleExtensions
{
    public static CommandPatternCodingStyleFeature CommandPattern(this CodingStyleConfigurator _) =>
        new();
}