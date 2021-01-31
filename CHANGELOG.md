# Changelog

All notable changes to this project will be documented in this file.

For each release both ASP.NET Core 3.X and ASP.NET Core 2.X packages are referenced.

## [Released]

## [4.2.0] - 2021-01-31

### Added

- Add option to __skip default__ area, controller and action values when possible.
```
services.AddSwaggerGenWithConventionalRoutes(options =>
{
    options.SkipDefaults = true;
});
```

## [Released]

## [4.1.0] - 2021-01-17

### Added

- Added support for __.NET 5.0__ framework

## [Released]

## [4.0.3] - 2020-12-26

### Added

- Define `SwaggerRoutingConfiguration` options during registration to ignore conventional routes based on their template

## [Released]

## [4.0.2] - 2020-12-15

### Added

- Add `SwaggerConfig` attribute and support for ignoring specific Controller Actions in swagger generation

## [4.0.1] - 2020-12-15

### Changed

- Remove api explorer dependency from netcore 3.x builds

## [4.0.0] - 2020-12-15

### Changed

- One package supports both ASP.NET Core 2.2 & ASP.NET Core 3.X applications
- Master branch is used for both ASP.NET Core 2.2 & ASP.NET Core 3.X builds
- Update original referenced `Swashbuckle.AspNetCore` to version `5.6.3`

## [3.0.1.7] , [2.2.1.8] - 2020-07-10

### Changed

- Use different tags for controllers with the same name but different area using **{area}.{controller}** format. This can always by overridden using the default `.SwaggerGen` options:

```csharp
services.AddSwaggerGen(c =>
{
    c.TagActionsBy(apiDesc =>
    {
        if (apiDesc.GroupName != null)
        {
            return new[] { apiDesc.GroupName };
        }

        var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;

        return new[] { $"MyCustomMessage{controllerActionDescriptor.ControllerName}" };
    });
});
```

## [3.0.1.6] , [2.2.1.7] - 2020-07-08

### Added

- Support area based controllers
