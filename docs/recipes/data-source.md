# Data Source

Data source recipe includes enough layers and feature implementations for a
backend application that is expected to provide data from configured data source
using report queries in `.sql` files.

To create an application from this recipe, use `DataSource()` extension of
`Bake` class directly in `Program.cs`.

```csharp
Bake.New
    .DataSource(
        business: c => c.DomainAssemblies([...])
    )
    .Run();
```

## Layers

| Name                 | Run                | Test               |
| -------------------- | ------------------ | ------------------ |
| Code Generation      | :white_check_mark: | :white_check_mark: |
| Data Access          | :white_check_mark: | :white_check_mark: |
| Domain               | :white_check_mark: | :white_check_mark: |
| HTTP Server          | :white_check_mark: | :no_entry:         |
| Rest API             | :white_check_mark: | :no_entry:         |
| Runtime              | :white_check_mark: | :white_check_mark: |
| Testing              | :no_entry:         | :white_check_mark: |

## Features

| Name               | Run                                | Test                               |
| ------------------ | ---------------------------------- | ---------------------------------- |
| Business           | :white_check_mark: (No Default)    | :white_check_mark:                 |
| Caching            | :white_check_mark: Scoped Memory   | :white_check_mark:                 |
| Coding Style(s)    | :white_check_mark:                 | :white_check_mark:                 |
|                    | Add/Remove Child                   |                                    |
|                    | Command Pattern                    |                                    |
|                    | Records are DTOs                   |                                    |
|                    | Remaining Services are Singleton   |                                    |
|                    | Scoped by Suffix                   |                                    |
|                    | Use Built-in Types                 |                                    |
|                    | Use Nullable Types                 |                                    |
|                    | With Method                        |                                    |
| Core               | :white_check_mark: Dotnet          | :white_check_mark: Mock            |
| Database           | :white_check_mark: Sqlite          | :white_check_mark: In Memory       |
| Exception Handling | :white_check_mark: Problem Details | :white_check_mark:                 |
| Greeting           | :white_check_mark: Swagger         | :no_entry:                         |
| Lifetime(s)        | :white_check_mark:                 | :white_check_mark:                 |
|                    | Singleton                          |                                    |
|                    | Scoped                             |                                    |
|                    | Transient                          |                                    |
| Logging            | :white_check_mark: Request         | :no_entry:                         |
| Mocking Overrider  | :no_entry:                         | :white_check_mark: First Interface |
| Reporting          | :white_check_mark: NativeSql       | :white_check_mark: Mock            |

> [!NOTE]
>
> When _Test_ column have :white_check_mark: without a note, this means it
> inherits whatever _Run_ column denotes.

## Phase Execution Order

```mermaid
flowchart TD
    CB(CreateBuilder)
    BC(BuildConfiguration)
    AD(AddDomainTypes)
    BD(BuildDomainModel)
    GC(GenerateCode)
    C(Compile)
    AS(AddServices)
    B(Build)
    PB(PostBuild)
    R(Run)

    CB -->|ConfigurationManager\nWebApplicationBuilder| BC
    BC --> AD
    AD -->|IDomainTypeCollection| BD
    BD -->|DomainModel| GC
    GC -->|IGeneratedAssemblyCollection| C
    C -->|GeneratedAssemblyProvider| AS
    AS -->|IServiceCollection| B
    B -->|IServiceProvider\nWebApplication|PB
    PB --> R
```