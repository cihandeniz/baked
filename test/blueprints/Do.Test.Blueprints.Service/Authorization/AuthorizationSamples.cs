﻿using Microsoft.AspNetCore.Authorization;

namespace Do.Test.Authorization;

public class AuthorizationSamples
{
    [Authorize]
    public void RequireAuthorization() { }

    [Authorize(Policy = "AdminOnly")]
    public void ClaimBasedAuthorization() { }
}