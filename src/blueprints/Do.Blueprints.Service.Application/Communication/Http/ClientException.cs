﻿namespace Do.Communication.Http;

public class ClientException(string content, HttpRequestException inner)
    : Exception(inner.Message, inner)
{
    public string Content { get; } = content;
    public new HttpRequestException InnerException { get; } = inner;
}

