# Spillgebees.NeTEx.Models

Pre-generated C# XML bindings for [NeTEx](https://github.com/NeTEx-CEN/NeTEx) (Network Timetable Exchange) XSD schemas.

Each version package provides strongly-typed classes for a specific [NeTEx-CEN/NeTEx release tag](https://github.com/NeTEx-CEN/NeTEx/tags). Install only the version you need, or use the meta-package to get all versions at once.

## Available versions

| NeTEx version | Package                           |                                                                              NuGet                                                                              |
|:-------------:|-----------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|     v1.2      | `Spillgebees.NeTEx.Models.V1_2`   |   [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2)   |
|    v1.2.2     | `Spillgebees.NeTEx.Models.V1_2_2` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_2?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_2) |
|    v1.2.3     | `Spillgebees.NeTEx.Models.V1_2_3` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_2_3?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_2_3) |
|    v1.3.0     | `Spillgebees.NeTEx.Models.V1_3_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_0) |
|    v1.3.1     | `Spillgebees.NeTEx.Models.V1_3_1` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V1_3_1?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V1_3_1) |
|    v2.0.0     | `Spillgebees.NeTEx.Models.V2_0_0` | [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models.V2_0_0?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models.V2_0_0) |
| All versions  | `Spillgebees.NeTEx.Models`        |        [![NuGet](https://img.shields.io/nuget/vpre/Spillgebees.NeTEx.Models?logo=nuget&label=)](https://www.nuget.org/packages/Spillgebees.NeTEx.Models)        |

## Getting started

```bash
# Install a single version
dotnet add package Spillgebees.NeTEx.Models.V1_3_1

# Or install all versions at once
dotnet add package Spillgebees.NeTEx.Models
```

## Usage

Each version package contains three sub-namespaces:

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

## Custom generation

If you need full control over namespaces or want to target a different NeTEx version, use the [`Spillgebees.Transmodel.Generator`](https://www.nuget.org/packages/Spillgebees.Transmodel.Generator) CLI tool to generate models from the XSD schemas directly.

## License

[EUPL-1.2](https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12). The NeTEx schemas are licensed under [GPL-3.0](https://github.com/NeTEx-CEN/NeTEx/blob/master/LICENSE) by CEN.
