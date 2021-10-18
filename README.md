# Wayfair.Text.Json

[![Wayfair.Text.Json Version](https://img.shields.io/badge/Wayfair.Text.Json-0.1.0-7f187f.svg)](https://github.com/wayfair-incubator/wayfair-text-json/blob/main/CHANGELOG.md)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg)](CODE_OF_CONDUCT.md)

## About this project

Extends the basic JSON Serialization/Deserialization functionality of [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-5.0).

`System.Text.Json` was built as a replacement for [Newtonsoft](https://www.newtonsoft.com/json), with a focus on performance. Both its .NET Core 3.1 and .NET 5 iterations, however, lack support for some desireable types. In .NET Core 3.1 in particular, there is noticeable missing support for dictionaries with non-string keys, quoted numbers, and specification of `DateFormatString` settings when handling `DateTime` and `DateTimeOffset`. While .NET 5 introduced support for dictionaries with non-string keys and quoted numbers, it still lacks support for specification of `DateFormatString` when handling `DateTime` and `DateTimeOffset`, so using this library may still be desirable for applications running .NET 5 and above.

### Notable Features

* Serialize and deserialize `Dictionary<TKey, TValue>` with non-string keys.
* Serialize and deserialize numbers represented by JSON strings (surrounded by quotes). For example, it can accept: `{"DegreesCelsius":"23"}` instead of `{"DegreesCelsius":23}`.
* Serialize and deserialize `DateTime` and `DateTimeOffset` with support for specification of `DateFormatString`.
* A `JsonBinder`, suitable for when you already have an existing object instance to bind JSON to.

## Installation

```sh
dotnet add package Wayfair.Text.Json
```

## Usage

There are three ways this package can be used:

1. Explicit calls to `WayfairJsonSerializer.Serialize` and `WayfairJsonSerializer.Deserialize` methods
2. Configuration of .NET's MVC framework using `.AddJsonOptions(...)` in startup
3. Calls to `JsonBinder.BindToObject` for scenarios when you already have an existing object instance on hand to bind values to

### WayfairJsonSerializer

Use this instead of calling the native `JsonSerializer` methods if you need to make an explicit call to `Serialize`/`Deserialize`:

```csharp
// To Serialize object to json:
var json = WayfairJsonSerializer.Serialize(obj);

// To Deserialize json to object of type T:
var obj = WayfairJsonSerializer.Deserialize<T>(json);

// or if you don't know the type at compile time: 
var obj = WayfairJsonSerializer.Deserialize(json, type);
```

### MVC

This is for automatic serialization for when data leaves a controller in .NET's MVC. To further customize, in your startup file, in the ConfigureServices method, chain `AddJsonOptions` after `AddMvcCore`:

```csharp
services
    .AddMvcCore()
    .AddJsonOptions(options =>
    {
        // You can choose which options to use. Here, we use the default ones set in WayfairJsonSerializer.
        options.JsonSerializerOptions.PropertyNamingPolicy = WayfairJsonSerializer.Options().PropertyNamingPolicy;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = WayfairJsonSerializer.Options().PropertyNameCaseInsensitive;
        
        // Add custom converters. Here, we add all the custom converters in WayfairJsonSerializer, then add another custom MyTypeConverter
        // You can add more, or filter some out by not adding them.
        foreach (var jsonConverter in WayfairJsonSerializer.Options().JsonConverters())
        {
            options.JsonSerializerOptions.Converters.Add(jsonConverter);
        }

        options.JsonSerializerOptions.Converters.Add(new MyTypeConverter());
    })   
    ...
```

### JsonBinder

If you already have an object instance on hand that you would like to bind some JSON to, using `JsonBinder` may be preferable:

```csharp
var dictionary = new Dictionary<string, string>();

const string json = @"
    {
        ""key_one"": ""value_one"",
        ""key_two"": ""value_two""
    }";

dictionary = JsonBinder.BindToObject(dictionary, json);
```

## Roadmap

See the [open issues](https://github.com/wayfair-incubator/wayfair-text-json/issues) for a list of proposed features (and known issues).

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**. For detailed contributing guidelines, please see [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Distributed under the `MIT` License. See `LICENSE` for more information.

## Contact

Maintainers: [MAINTAINERS.md](MAINTAINERS.md)

Project Link: [https://github.com/wayfair-incubator/wayfair-text-json](https://github.com/wayfair-incubator/wayfair-text-json)

## Acknowledgements

This README was adapted from
[https://github.com/othneildrew/Best-README-Template](https://github.com/othneildrew/Best-README-Template).

## References

* [Microsoft Devblog about System.Text.Json](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/)
* [Microsoft Docs on System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=netcore-3.0)
* [Example Custom Converters](https://github.com/steveharter/dotnet_corefx/tree/d5e447f1d998b42c1a87258dddceb9aaf35ebe8b/src/System.Text.Json/tests/Serialization)
* [Examples discussion](https://github.com/dotnet/corefx/issues/36639)
* [Microsoft Docs on how to Write Custom Converters](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?view=netcore-3.1)
