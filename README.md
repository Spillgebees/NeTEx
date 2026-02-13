# Spillgebees.Transmodel

<p align="center">
    <img alt="GitHub Workflow Status (with branch)" src="https://img.shields.io/github/actions/workflow/status/spillgebees/transmodel/build-and-test.yml?branch=main&label=build%20%26%20test&style=for-the-badge" />
    <img alt="License" src="https://img.shields.io/badge/license-EUPL--1.2-blue?style=for-the-badge" />
</p>

Strongly-typed C# XML bindings for [Transmodel](https://transmodel-cen.eu/)-based European public transport standards. Currently, covers [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) and [SIRI](https://github.com/SIRI-CEN/SIRI) (Service Interface for Real-time Information), with the flexibility to support additional Transmodel-based standards.

Generated from the official CEN XSD schemas and verified with automated tests.

> **Note:** This project was developed with significant AI assistance. While it is functional and tested, it has not yet undergone a full manual review. Treat it as pre-production. Cntributions and feedback are welcome.

## Generator tool

A CLI tool that downloads XSD schemas from the official CEN repositories and generates modern C# XML bindings with the latest features. Use it to generate bindings for any schema version, including unreleased branches or specific commits.

| Package                              |                                                                                    NuGet                                                                                    |
|--------------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| `Spillgebees.Transmodel.Generator`   |      [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.Transmodel.Generator?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.Transmodel.Generator)      |

```bash
dotnet tool install -g Spillgebees.Transmodel.Generator
```

### NeTEx generation

```bash
transmodel-generator generate-netex --version v1.3.1 --output ./Generated --namespace MyApp.NeTEx
```

Generates XML bindings with the following sub-namespaces:

| Sub-namespace       | XML Namespace                    | Description                            |
|---------------------|----------------------------------|----------------------------------------|
| `MyApp.NeTEx.NeTEx` | `http://www.netex.org.uk/netex`  | NeTEx types                            |
| `MyApp.NeTEx.SIRI`  | `http://www.siri.org.uk/siri`    | SIRI types (subset bundled with NeTEx) |
| `MyApp.NeTEx.GML`   | `http://www.opengis.net/gml/3.2` | GML types (geographic markup)          |

### SIRI generation

```bash
transmodel-generator generate-siri --version v2.2 --output ./Generated --namespace MyApp.SIRI
```

Generates XML bindings with the following sub-namespaces:

| Sub-namespace       | XML Namespace                          | Description                   |
|---------------------|----------------------------------------|-------------------------------|
| `MyApp.SIRI.SIRI`   | `http://www.siri.org.uk/siri`          | Core SIRI types               |
| `MyApp.SIRI.IFOPT`  | `http://www.ifopt.org.uk/ifopt`        | IFOPT types                   |
| `MyApp.SIRI.ACSB`   | `http://www.ifopt.org.uk/acsb`         | Accessibility types           |
| `MyApp.SIRI.DATEX2` | `http://datex2.eu/schema/2_0RC1/2_0`   | DATEX2 types                  |
| `MyApp.SIRI.WSDL`   | `http://wsdl.siri.org.uk`              | WSDL/SOAP types               |
| `MyApp.SIRI.GML`    | `http://www.opengis.net/gml/3.2`       | GML types (geographic markup) |
| `MyApp.SIRI.W3`     | `http://www.w3.org/XML/1998/namespace` | W3 types                      |

### CLI reference

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

## Pre-generated packages

If you don't need custom generation, install one of the pre-generated packages below. These are built with the same generator, tested, and published to NuGet for every release.

### NeTEx

Versions correspond to [tags in the NeTEx-CEN/NeTEx GitHub repository](https://github.com/NeTEx-CEN/NeTEx/tags).

|                      NeTEx version                       | Package                           |                                                                              NuGet                                                                              |
|:--------------------------------------------------------:|-----------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|   [v1.2](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2)   | `Spillgebees.NeTEx.Models.V1_2`   |   [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2)   |
| [v1.2.2](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2.2) | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
| [v1.2.3](https://github.com/NeTEx-CEN/NeTEx/tree/v1.2.3) | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
| [v1.3.0](https://github.com/NeTEx-CEN/NeTEx/tree/v1.3.0) | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
| [v1.3.1](https://github.com/NeTEx-CEN/NeTEx/tree/v1.3.1) | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
|                       All versions                       | `Spillgebees.NeTEx.Models`        |        [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models)        |

```bash
dotnet add package Spillgebees.NeTEx.Models.V1_3_1
```

Each NeTEx version package contains three sub-namespaces:

| Sub-namespace | XML Namespace                    | Description                            |
|---------------|----------------------------------|----------------------------------------|
| `.NeTEx`      | `http://www.netex.org.uk/netex`  | NeTEx types                            |
| `.SIRI`       | `http://www.siri.org.uk/siri`    | SIRI types (subset bundled with NeTEx) |
| `.GML`        | `http://www.opengis.net/gml/3.2` | GML types (geographic markup)          |

```csharp
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Spillgebees.NeTEx.Models.V1_3_1.NeTEx;

var delivery = new PublicationDeliveryStructure
{
    PublicationTimestamp = DateTimeOffset.UtcNow,
    ParticipantRef = "my-data-provider",
    Description = new MultilingualString { Value = "Stop places export" },
};

var serializer = new XmlSerializer(typeof(PublicationDeliveryStructure));
using var stream = new MemoryStream();
using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
{
    Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
    Indent = true,
});
serializer.Serialize(xmlWriter, delivery);

// <?xml version="1.0" encoding="utf-8"?>
// <PublicationDelivery xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ...>
//   <PublicationTimestamp>2026-02-13T12:00:00+00:00</PublicationTimestamp>
//   <ParticipantRef>my-data-provider</ParticipantRef>
//   <Description>Stop places export</Description>
// </PublicationDelivery>
```

### SIRI

Versions correspond to [tags in the SIRI-CEN/SIRI GitHub repository](https://github.com/SIRI-CEN/SIRI/tags).

|                    SIRI version                    | Package                        |                                                                           NuGet                                                                           |
|:--------------------------------------------------:|--------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------:|
| [v2.1](https://github.com/SIRI-CEN/SIRI/tree/v2.1) | `Spillgebees.SIRI.Models.V2_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models.V2_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models.V2_1) |
| [v2.2](https://github.com/SIRI-CEN/SIRI/tree/v2.2) | `Spillgebees.SIRI.Models.V2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models.V2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models.V2_2) |
|                    All versions                    | `Spillgebees.SIRI.Models`      |      [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.SIRI.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.SIRI.Models)      |

```bash
dotnet add package Spillgebees.SIRI.Models.V2_2
```

Each SIRI version package contains seven sub-namespaces:

| Sub-namespace | XML Namespace                          | Description                   |
|---------------|----------------------------------------|-------------------------------|
| `.SIRI`       | `http://www.siri.org.uk/siri`          | SIRI types                    |
| `.IFOPT`      | `http://www.ifopt.org.uk/ifopt`        | IFOPT types                   |
| `.ACSB`       | `http://www.ifopt.org.uk/acsb`         | Accessibility types           |
| `.DATEX2`     | `http://datex2.eu/schema/2_0RC1/2_0`   | DATEX2 types                  |
| `.WSDL`       | `http://wsdl.siri.org.uk`              | WSDL/SOAP types               |
| `.GML`        | `http://www.opengis.net/gml/3.2`       | GML types (geographic markup) |
| `.W3`         | `http://www.w3.org/XML/1998/namespace` | W3 types                      |

```csharp
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Spillgebees.SIRI.Models.V2_2.SIRI;

var siri = new Siri
{
    ServiceDelivery = new ServiceDelivery
    {
        ResponseTimestamp = DateTimeOffset.UtcNow,
    },
};

var serializer = new XmlSerializer(typeof(Siri));
using var stream = new MemoryStream();
using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
{
    Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
    Indent = true,
});
serializer.Serialize(xmlWriter, siri);

// <?xml version="1.0" encoding="utf-8"?>
// <Siri xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ...>
//   <ServiceDelivery>
//     <ResponseTimestamp>2026-02-13T12:00:00+00:00</ResponseTimestamp>
//   </ServiceDelivery>
// </Siri>
```

## Building from source

The generated bindings are **not committed** to the repository, they are generated at build time. To build locally:

```bash
dotnet build Spillgebees.Transmodel.slnx --configuration Release
dotnet test --solution Spillgebees.Transmodel.slnx --configuration Release
```

The build downloads the XSD schemas from GitHub, generates the XML bindings, and compiles them. Downloaded schemas are cached in local app data so subsequent builds don't require network access.

Use `dotnet clean` to remove the generated files and trigger a fresh generation on the next build.

## Supported frameworks

- .NET 10.0

## License

This project is licensed under the [European Union Public Licence v. 1.2 (EUPL-1.2)](LICENSE).

The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN. EUPL-1.2 is [compatible](https://joinup.ec.europa.eu/collection/eupl/matrix-eupl-compatible-open-source-licences) with GPL-3.0 per its compatibility clause. The SIRI schemas are published by CEN on the [SIRI-CEN/SIRI](https://github.com/SIRI-CEN/SIRI) repository without an explicit open-source license.
