# Unreleased

# Breaking Changes

- `CodeGeneration` namespace is changed to `Builtime`, along with its layer name
  and extensions
  - `configurator.CodeGeneration` is now `configurator.Buildtime`
  - `CodeGenerationLayer` is now `BuildtimeLayer`
  - Anything under `Baked.CodeGeneration` is now under `Baked.Buildtime`
- `CodeGeneration.GenerateCode` phase is now renamed as `Buildtime.Generate`
- `DescriptorBuilderAttribute` and `ComponentDescriptorBuilderAttribute` are
  renamed as `GeneratorAttribute` and `ComponentGeneratorAttribute` respectively
  - `Build` method is renamed as `Generate`
- `GetSchema`, `GetSchemas`, `GetRequiredSchema`, `GetComponent` and
  `GetRequiredComponent` extension methods are renamed as `GenerateSchema`,
  `GenerateSchemas`, `GenerateRequiredSchema`, `GenerateComponent` and
  `GenerateRequiredComponent` respectively
- `FormPage` schema is completely redesigned, migrate your existing
  configurations to match the new one
- `GroupAttribute` now supports context based names through `this[string
  context]` indexer property
  - Use extension properties to set/get a group name for your custom context,
    see
    ```csharp
    extension(GroupAttribute group)
    {
        public string MyCustomName { get => group["MyCustom"]; set => group["MyCustom"] = value; }
    }
    ```
- `TabNameAttribute` is now removed, instead get `GroupAttribute` and use its
  `TabName` extension property

## Improvements

- Inspection mechanism
  - Add trace wasn't showing up when initial value is null, fixed
  - JSON serialization is restricted to only anonymous types to avoid
    unnecessarily long (and for some attributes failing) serializations
