# Unreleased

## Features

- `Business` namespace now provides `Validate` object to perform business
  validations
- `FlagsEnumCodingStyle` is now available that supports enums with `[Flags]`
- `IExportOptions` is introduced in `Baked.Binding` so that an attribute
  can control the name it is exported with, e.g., `IdProperty` attribute is
  exported as `@id`
  - Attributes named after their target do not carry that noise to exports,
    `CommandMethod` is exported as `@command`, `MappedMethod` as `@mapped` and
    `QueryMethod` as `@query`

## Breaking Changes

- Attributes that are used in conventions are renamed, the `Attribute`
  suffix is removed and some are renamed further to avoid clashes
  | from                             | to                           |
  | -------------------------------- | ---------------------------- |
  | `ActionAttribute`                | `UiAction`                   |
  | `ActionModelAttribute`           | `ApiAction`                  |
  | `AllowAnonymousAttribute`        | `AllowAnonymous`             |
  | `AllParametersAreApiInput()`     | `AllParametersAreBindable()` |
  | `ApiInputAttribute`              | `Bindable`                   |
  | `ClientAttribute`                | `Client`                     |
  | `ClientCacheAttribute`           | `ClientCache`                |
  | `ColumnAttribute`                | `Column`                     |
  | `CommandAttribute`               | `Command`                    |
  | `CommandMethodAttribute`         | `CommandMethod`              |
  | `ComponentGeneratorAttribute<T>` | `ComponentGenerator<T>`      |
  | `ContextBasedComponent.Filter`   | `FilterDelegate`             |
  | `ContextBasedComponentAttribute` | `ContextBasedComponent`      |
  | `ControllerModelAttribute`       | `ApiController`              |
  | `DataAttribute`                  | `UiData`                     |
  | `DescriptionAttribute`           | `UiDescription`              |
  | `EntityAttribute`                | `Entity`                     |
  | `ExternalAttribute`              | `External`                   |
  | `ForeignKeyAttribute`            | `ForeignKey`                 |
  | `GeneratorAttribute<T>`          | `Generator<T>`               |
  | `GroupAttribute`                 | `Group`                      |
  | `IdAttribute`                    | `IdProperty`                 |
  | `InitializerAttribute`           | `Initializer`                |
  | `IsApiInput`                     | `IsBindable`                 |
  | `LabelAttribute`                 | `Label`                      |
  | `LocatableAttribute`             | `Locatable`                  |
  | `LocatableExtensionAttribute`    | `LocatableExtension`         |
  | `MappedMethodAttribute`          | `MappedMethod`               |
  | `NamespaceAttribute`             | `Namespace`                  |
  | `NoTransactionAttribute`         | `NoTransaction`              |
  | `ObjectWithListAttribute`        | `ObjectWithList`             |
  | `PagingAttribute`                | `Paging`                     |
  | `ParameterModelAttribute`        | `ApiParameter`               |
  | `QueryAttribute`                 | `Query`                      |
  | `QueryMethodAttribute`           | `QueryMethod`                |
  | `RequireUserAttribute`           | `RequireUser`                |
  | `RichTransientAttribute`         | `Resource`                   |
  | `RouteAttribute`                 | `UiRoute`                    |
  | `ScopedAttribute`                | `Scoped`                     |
  | `ServiceAttribute`               | `Service`                    |
  | `SingletonAttribute`             | `Singleton`                  |
  | `SortingAttribute`               | `Sorting`                    |
  | `TransientAttribute`             | `Transient`                  |
  | `TryGetLocatableAttribute()`     | `TryGetLocatable()`          |
  | `UniqueAttribute`                | `Unique`                     |
  | `ValueTypeAttribute`             | `Primitive`                  |
  - `Bindable` is moved from `Baked.RestApi.Model` to `Baked.Binding`, since it
    marks types that can be bound from a request and is only used by rest
    binding
  - `Primitive` avoids clashing with `System.ValueType`
  - `Resource` is used since transients are already rich, this coding style
    only makes them locatable by their id
  - `Generator<T>` renames its `Generator` and `Filter` properties as
    `GeneratorDelegate` and `FilterDelegate`
  - Names in exported `.kdl` files do not change, the `Attribute` suffix was
    already being stripped during export
- `AllowAnonymous`, `ClientCache` and `NoTransaction` attributes now declare
  `[AttributeUsage]`, so they are no longer included in every export target
- Coding styles are renamed to express how they detect types, `via` is used when
  the mechanism needs naming and `based` when it reads as a qualifier
  | from                 | to                                 |
  | -------------------- | ---------------------------------- |
  | `AddRemoveChild`     | `AddRemoveChildAsSubResource`      |
  | `Client`             | `SuffixBasedClient`                |
  | `CommandPattern`     | `CommandViaMethodName`             |
  | `Id`                 | `TypeBasedId`                      |
  | `Initializable`      | `InitializableViaMethodName`       |
  | `Label`              | `NameBasedLabel`                   |
  | `Locatable`          | `LocateViaId`                      |
  | `LocatableExtension` | `ExtensionViaLocatableInitializer` |
  | `Query`              | `QueryViaPluralName`               |
  | `RichTransient`      | `ResourceViaIdInitializer`         |
  | `ScopedBySuffix`     | `ScopedViaSuffix`                  |
  | `Unique`             | `UniqueViaSingleBy`                |
  | `ValueType`          | `PrimitiveViaParsable`             |
  - To migrate, use the new names in `AddCodingStyles()`, e.g.,
    `c => c.ValueType()` -> `c => c.PrimitiveViaParsable()`
  - `RichEntity` and `FlagsEnum` are kept as they are
- `MonolithRecipe` and `DataSourceRecipe` configuration methods follow their
  coding styles, e.g., `CommandPattern(...)` -> `CommandViaMethodName(...)`
- `ValueTypeUserType<T>` -> `PrimitiveUserType<T>`
- `EntityInitializerIsPostResourceConvention` ->
  `EntityInitializerIsPostConvention`
- Conventions that configure an existing attribute are renamed as edits, and
  are now regular conventions added via `conventions.Add()`
  | from                                      | to                            |
  | ----------------------------------------- | ----------------------------- |
  | `AddTypeAttributeConfiguration<T>()`      | `EditTypeAttribute<T>()`      |
  | `AddPropertyAttributeConfiguration<T>()`  | `EditPropertyAttribute<T>()`  |
  | `AddMethodAttributeConfiguration<T>()`    | `EditMethodAttribute<T>()`    |
  | `AddParameterAttributeConfiguration<T>()` | `EditParameterAttribute<T>()` |
  | `AddTypeComponentConfiguration<T>()`      | `EditTypeComponent<T>()`      |
  | `AddPropertyComponentConfiguration<T>()`  | `EditPropertyComponent<T>()`  |
  | `AddMethodComponentConfiguration<T>()`    | `EditMethodComponent<T>()`    |
  | `AddParameterComponentConfiguration<T>()` | `EditParameterComponent<T>()` |
  | `AddTypeSchemaConfiguration<T>()`         | `EditTypeSchema<T>()`         |
  | `AddPropertySchemaConfiguration<T>()`     | `EditPropertySchema<T>()`     |
  | `AddMethodSchemaConfiguration<T>()`       | `EditMethodSchema<T>()`       |
  | `AddParameterSchemaConfiguration<T>()`    | `EditParameterSchema<T>()`    |
- `Label` component is renamed as `Labeler`
- `QueryMethodCodingStyle` is merged into `QueryCodingStyle`, which is now
  `QueryViaPluralNameCodingStyle`
- Lifetime features are renamed after the lifetime they provide
  | from        | to            |
  | ----------- | ------------- |
  | `Singleton` | `Application` |
  | `Scoped`    | `Scope`       |
  | `Transient` | `Instance`    |
- Domain components are removed, all conventions now come from
  `DefaultThemeFeature` by default
  - To migrate, just use components directly instead of through domain
    components, e.g., `TypeFormPage` -> `B.FormPage()`
- `EntitySubclassCodingStyle` is removed completely
- `ICasts` interface and `Caster.Cast()` extension are removed
- `ExtensionViaLocatableInitializerCodingStyle` now does not require extension
  classes to have an implicit operator
  - Any transient with an initializer method that has one parameter that is
    locatable, e.g., `internal MyExtension With(MyLocatable locatable) { ... }`,
    becomes an extension for that locatable
  - This might result unintended classes to become an extension causing their
    API endpoint routes to change, removing `LocatableExtension` attribute
    from unwanted classes, or adding another parameter to the initializer,
    will resolve the issue
