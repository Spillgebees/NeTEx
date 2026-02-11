# Spillgebees.NeTEx.Generator

CLI tool for generating C# model classes from [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) XSD schemas.

Downloads schemas from the official [NeTEx-CEN/NeTEx](https://github.com/NeTEx-CEN/NeTEx) repository and generates modern C# code ready to use with `XmlSerializer`. Downloaded schemas are cached locally so repeated runs don't hit GitHub.

## Getting started

Install the tool globally:

```bash
dotnet tool install -g Spillgebees.NeTEx.Generator
```

Generate models for a specific NeTEx version tag:

```bash
netex-generate generate --version v1.3.1 --output ./Generated --namespace MyApp.NeTEx
```

This downloads the NeTEx schemas and generates C# classes with the following sub-namespaces:

- `MyApp.NeTEx.NeTEx` -- NeTEx types (stop places, lines, journeys, fares, etc.)
- `MyApp.NeTEx.SIRI` -- SIRI types (real-time information)
- `MyApp.NeTEx.GML` -- GML types (geographic markup)

## Schema caching

Schemas are cached locally after the first download. Version tags and commit SHAs are immutable, so they are cached permanently. Branch refs are always re-downloaded.

The cache is stored in local app data under `netex-schemas/`:

- **Linux**: `~/.local/share/netex-schemas/`
- **macOS**: `~/Library/Application Support/netex-schemas/`
- **Windows**: `%LOCALAPPDATA%\netex-schemas\`

## CLI reference

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

## Pre-generated models

If you don't need custom generation, use the pre-generated model packages instead:

| NeTEx version | Package                           |                                                                              NuGet                                                                              |
|:-------------:|-----------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|     v1.2      | `Spillgebees.NeTEx.Models.V1_2`   |   [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2)   |
|    v1.2.2     | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
|    v1.2.3     | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
|    v1.3.0     | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
|    v1.3.1     | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
| All versions  | `Spillgebees.NeTEx.Models`        |        [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models)        |

## License

[EUPL-1.2](https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12). The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN.
