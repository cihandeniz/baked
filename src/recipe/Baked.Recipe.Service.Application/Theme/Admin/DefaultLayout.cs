﻿using Baked.Ui;

namespace Baked.Theme.Admin;

public record DefaultLayout(string Path)
    : IGeneratedComponentSchema
{
    public string Path { get; set; } = Path;
    public IComponentDescriptor? SideMenu { get; set; }
    public IComponentDescriptor? Header { get; set; }
}