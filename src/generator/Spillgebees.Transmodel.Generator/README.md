# Spillgebees.Transmodel.Generator

CLI tool for generating C# XML bindings from [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) and [SIRI](https://github.com/SIRI-CEN/SIRI) (Service Interface for Real-time Information) XSD schemas.

Downloads schemas from the official repositories and generates modern C# XML bindings. Downloaded schemas are cached locally so repeated runs don't hit GitHub.

## Getting started

Install the tool globally:

```bash
dotnet tool install -g Spillgebees.Transmodel.Generator
```

### NeTEx generation

Generate bindings for a specific NeTEx version tag:

```bash
transmodel-generator generate-netex --version v1.3.1 --output ./Generated --namespace MyApp.NeTEx
```

This downloads the NeTEx schemas and generates XML bindings with the following sub-namespaces:

| Sub-namespace        | XML Namespace                    | Description                            |
|----------------------|----------------------------------|----------------------------------------|
| `MyApp.NeTEx.NeTEx`  | `http://www.netex.org.uk/netex`  | NeTEx types                            |
| `MyApp.NeTEx.SIRI`   | `http://www.siri.org.uk/siri`    | SIRI types (subset bundled with NeTEx) |
| `MyApp.NeTEx.GML`    | `http://www.opengis.net/gml/3.2` | GML types (geographic markup)          |

### SIRI generation

Generate bindings for a specific SIRI version tag:

```bash
transmodel-generator generate-siri --version v2.2 --output ./Generated --namespace MyApp.SIRI
```

This downloads the SIRI schemas and generates XML bindings with the following sub-namespaces:

| Sub-namespace        | XML Namespace                          | Description                   |
|----------------------|----------------------------------------|-------------------------------|
| `MyApp.SIRI.SIRI`    | `http://www.siri.org.uk/siri`          | Core SIRI types               |
| `MyApp.SIRI.IFOPT`   | `http://www.ifopt.org.uk/ifopt`        | IFOPT types                   |
| `MyApp.SIRI.ACSB`    | `http://www.ifopt.org.uk/acsb`         | Accessibility types           |
| `MyApp.SIRI.DATEX2`  | `http://datex2.eu/schema/2_0RC1/2_0`   | DATEX2 types                  |
| `MyApp.SIRI.WSDL`    | `http://wsdl.siri.org.uk`              | WSDL/SOAP types               |
| `MyApp.SIRI.GML`     | `http://www.opengis.net/gml/3.2`       | GML types (geographic markup) |
| `MyApp.SIRI.W3`      | `http://www.w3.org/XML/1998/namespace` | W3 types                      |

## Schema caching

Schemas are cached locally after the first download. Version tags and commit SHAs are immutable, so they are cached permanently. Branch refs are always re-downloaded.

The cache is stored in local app data:

- **Linux**: `~/.local/share/{netex,siri}-schemas/`
- **macOS**: `~/Library/Application Support/{netex,siri}-schemas/`
- **Windows**: `%LOCALAPPDATA%\{netex,siri}-schemas\`

## CLI reference

```
transmodel-generator generate-netex [options]
transmodel-generator generate-siri  [options]

Options:
  -v, --version <version>      Schema version tag (default: v1.3.1 for NeTEx, v2.2 for SIRI)
  --ref <ref>                  Git ref (branch or commit SHA), mutually exclusive with --version
  -o, --output <output>        Output directory for generated C# files (default: ./Generated)
  -n, --namespace <namespace>  Root C# namespace (default: NeTEx.Models / SIRI.Models)
  --clean                      Delete output directory before generating
  --verbose                    Enable verbose logging

transmodel-generator list-netex-versions   List available NeTEx version tags
transmodel-generator list-siri-versions    List available SIRI version tags
```

## Pre-generated models

If you don't need custom generation, use the pre-generated packages instead:

### NeTEx

| NeTEx version | Package                           |                                                                              NuGet                                                                              |
|:-------------:|-----------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|     v1.2      | `Spillgebees.NeTEx.Models.V1_2`   |   [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2)   |
|    v1.2.2     | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
|    v1.2.3     | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
|    v1.3.0     | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
|    v1.3.1     | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
|    v2.0.0     | `Spillgebees.NeTEx.Models.V2_0_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V2_0_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V2_0_0) |
| All versions  | `Spillgebees.NeTEx.Models`        |        [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models)        |

### SIRI

| SIRI version | Package                          |                                                                            NuGet                                                                             |
|:------------:|----------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|     v2.1     | `Spillgebees.SIRI.Models.V2_1`   | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models.V2_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models.V2_1)   |
|     v2.2     | `Spillgebees.SIRI.Models.V2_2`   | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models.V2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models.V2_2)   |
| All versions | `Spillgebees.SIRI.Models`        |      [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models)        |

## License

[EUPL-1.2](https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12). The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN. The SIRI schemas are published by CEN on the [SIRI-CEN/SIRI](https://github.com/SIRI-CEN/SIRI) repository without an explicit open-source license.
