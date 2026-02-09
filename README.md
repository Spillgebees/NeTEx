# Spillgebees.NeTEx

> **Note:** This project was mostly AI-generated and is still a work in progress. It is not ready for production use.

<p align="center">
    <img alt="GitHub Workflow Status (with branch)" src="https://img.shields.io/github/actions/workflow/status/spillgebees/netex-csharp-model/build-and-test.yml?branch=main&label=build%20%26%20test&style=for-the-badge" />
</p>

C# model classes and a generator tool for [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) XSD schemas, for use in .NET applications.

This repository provides two things:

- **Pre-generated model packages** -- strongly-typed C# classes for each NeTEx schema version (as [tagged in the NeTEx-CEN/NeTEx repository](https://github.com/NeTEx-CEN/NeTEx/tags)), ready to use with `XmlSerializer`.
- **A generator tool** -- a CLI (`netex-generate`) that downloads NeTEx XSD schemas and generates C# classes with custom namespaces, for when you need full control.

## Model packages

Install only the NeTEx version you need. Versions correspond to [tags in the NeTEx-CEN/NeTEx GitHub repository](https://github.com/NeTEx-CEN/NeTEx/tags).

| NeTEx version | Package | NuGet |
|:---:|---|:---:|
| [v1.2](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2) | `Spillgebees.NeTEx.Models.V1_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2) |
| [v1.2.2](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2.2) | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
| [v1.2.3](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2.3) | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
| [v1.3.0](https://github.com/NeTEx-CEN/NeTEx/tree/v1.3.0) | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
| [v1.3.1](https://github.com/NeTEx-CEN/NeTEx/tree/v1.3.1) | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
| All versions | `Spillgebees.NeTEx.Models` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models) |

## Generator tool

| Package | NuGet |
|---|:---:|
| `Spillgebees.NeTEx.Generator` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Generator?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Generator) |

## Quick start

### Using the pre-generated models

```bash
# Install a single version
dotnet add package Spillgebees.NeTEx.Models.V1_3_1

# Or install all versions at once
dotnet add package Spillgebees.NeTEx.Models
```

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

### Using the generator tool

Install the tool globally:

```bash
dotnet tool install -g Spillgebees.NeTEx.Generator
```

Generate models for a specific [NeTEx-CEN/NeTEx](https://github.com/NeTEx-CEN/NeTEx) version tag:

```bash
netex-generate generate --version v1.3.1 --output ./Generated --namespace MyApp.NeTEx
```

This downloads the NeTEx schemas from the official repository and generates C# classes with the following sub-namespaces:

- `MyApp.NeTEx.NeTEx` -- NeTEx types
- `MyApp.NeTEx.SIRI` -- SIRI types
- `MyApp.NeTEx.GML` -- GML types

#### Generator CLI reference

```
netex-generate generate [options]

Options:
  -v, --version <version>      NeTEx-CEN/NeTEx version tag (default: v1.3.1)
  --ref <ref>                  Git ref (branch or commit SHA), mutually exclusive with --version
  -o, --output <output>        Output directory for generated C# files (default: ./Generated)
  -n, --namespace <namespace>  Root C# namespace (default: NeTEx.Models)
  --clean                      Delete output directory before generating
  --verbose                    Enable verbose logging

netex-generate list-versions   List available NeTEx-CEN/NeTEx version tags
```

## Building from source

The generated model classes are **not committed** to the repository -- they are generated at build time. To build locally:

```bash
# 1. Build the generator
dotnet build src/Spillgebees.NeTEx.Generator --configuration Release

# 2. Generate models for each NeTEx-CEN/NeTEx version tag
for tag_ns in "v1.2 V1_2" "v1.2.2 V1_2_2" "v1.2.3 V1_2_3" "v1.3.0 V1_3_0" "v1.3.1 V1_3_1"; do
  set -- $tag_ns
  dotnet run --project src/Spillgebees.NeTEx.Generator --configuration Release -- \
    generate --version "$1" --output "src/Spillgebees.NeTEx.Models.$2/Generated" \
    --namespace "Spillgebees.NeTEx.Models.$2"
done

# 3. Build and test
dotnet build Spillgebees.NeTEx.Models.slnx --configuration Release
dotnet test Spillgebees.NeTEx.Models.slnx --configuration Release
```

## Supported frameworks

- .NET 10.0

## License

This project is licensed under the [European Union Public Licence v. 1.2 (EUPL-1.2)](LICENSE).

The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN. EUPL-1.2 is [compatible](https://joinup.ec.europa.eu/collection/eupl/matrix-eupl-compatible-open-source-licences) with GPL-3.0 per its compatibility clause.
