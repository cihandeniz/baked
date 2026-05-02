# Unreleased

# Breaking Changes

- `CodeGeneration` namespace is changed to `Builtime`, along with its layer name
  and extensions
  - `configurator.CodeGeneration` is now `configurator.Buildtime`
  - `CodeGenerationLayer` is now `BuildtimeLayer`
  - Anything under `Baked.CodeGeneration` is now under `Baked.Buildtime`
- `DescriptorBuilderAttribute` and `ComponentDescriptorBuilderAttribute` are
  renamed as `GeneratorAttribute` and `ComponentGeneratorAttribute` respectively
  - `Build` method is renamed as `Generate`
- `GetSchema`, `GetSchemas`, `GetRequiredSchema`, `GetComponent` and
  `GetRequiredComponent` extension methods are renamed as `GenerateSchema`,
  `GenerateSchemas`, `GenerateRequiredSchema`, `GenerateComponent` and
  `GenerateRequiredComponent` respectively
- `FormPage` schema is completely redesigned, migrate your existing
  configurations to match the new one
- `TabNameAttribute` is now removed, use `GroupAttribute` instead

## Improvements

- Inspection mechanism
  - It was not tracing changes from parent components, fixed
  - Add trace wasn't showing up when initial value is null, fixed
  - JSON serialization is restricted to only anonymous types to avoid
    unnecessarily long (and for some attributes failing) serializations
