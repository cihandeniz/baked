﻿using Baked.RestApi.Configuration;

namespace Baked.Authorization.ClaimBased;

public class RequireUserIsAuthorizeConvention : IApiModelConvention<ActionModelContext>
{
    public void Apply(ActionModelContext context)
    {
        if (context.Action.MappedMethod is null) { return; }
        if (!context.Action.MappedMethod.Has<RequireUserAttribute>()) { return; }

        context.Action.AdditionalAttributes.Add("Authorize");
    }
}