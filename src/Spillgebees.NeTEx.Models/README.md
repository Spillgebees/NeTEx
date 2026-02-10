# Spillgebees.NeTEx.Models

Pre-generated C# model classes for [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) XSD schemas, ready to use with `XmlSerializer`.

Each version package provides strongly-typed classes for a specific [NeTEx-CEN/NeTEx release tag](https://github.com/NeTEx-CEN/NeTEx/tags). Install only the version you need, or use the meta-package to get all versions at once.

## Available versions

| NeTEx version | Package | NuGet |
|:---:|---|:---:|
| v1.2 | `Spillgebees.NeTEx.Models.V1_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2) |
| v1.2.2 | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
| v1.2.3 | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
| v1.3.0 | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
| v1.3.1 | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
| All versions | `Spillgebees.NeTEx.Models` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models) |

## Getting started

```bash
# Install a single version
dotnet add package Spillgebees.NeTEx.Models.V1_3_1

# Or install all versions at once
dotnet add package Spillgebees.NeTEx.Models
```

## Usage

Each version package contains three sub-namespaces:

- `.NeTEx` -- NeTEx types (stop places, lines, journeys, fares, etc.)
- `.SIRI` -- SIRI types (real-time information)
- `.GML` -- GML types (geographic markup)

```csharp
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;
using Spillgebees.NeTEx.Models.V1_3_1.SIRI;

var stopPlace = new StopPlace
{
    Id = "NSR:StopPlace:1234",
    Version = "1",
};
```

## Custom generation

If you need full control over namespaces or want to target a different NeTEx version, use the [`Spillgebees.NeTEx.Generator`](https://www.nuget.org/packages/Spillgebees.NeTEx.Generator) CLI tool to generate models from the XSD schemas directly.

## License

[EUPL-1.2](https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12). The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN.
