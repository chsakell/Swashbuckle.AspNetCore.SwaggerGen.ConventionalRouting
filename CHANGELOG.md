# Changelog

All notable changes to this project will be documented in this file.

For each release both ASP.NET Core 3.X and ASP.NET Core 2.X packages are referenced.

## [Released]

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
