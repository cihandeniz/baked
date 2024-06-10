﻿using System.Net;

namespace Baked.ExceptionHandling.Default;

public class UnauthorizedAccessExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is UnauthorizedAccessException;
    public ExceptionInfo Handle(Exception ex) => new(ex, (int)HttpStatusCode.Forbidden, ex.Message);
}