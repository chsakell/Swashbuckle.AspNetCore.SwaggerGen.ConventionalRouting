## Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting

Swagger generator extension for conventional based routes support.

# Getting Started #

1. Install the standard Nuget package into your ASP.NET Core application.

    ```
    Package Manager : Install-Package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting -Version 2.2.1.1
    CLI : dotnet add package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting --version 2.2.1.1
    ```
> The extension has a dependency on the [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) so you don't have to install it twice.


2. In the `ConfigureServices` method of `Startup.cs`, register the [default](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen) Swagger generator and then add the Conventional Routing Swagger generator.
    ```csharp
    using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;
    ```
    
    ```csharp
    services.AddMvc();
    services.AddSwaggerGen();
    
    // this will handle all conventional based routes
    services.AddSwaggerGenWithConventionalRoutes();
    ```
    
 3. In the `Configure` method of `Startup.cs`, after registering all your conventional routes, pass them as the argument to the Conventional Routing generator.
    
    ```csharp
    app.UseMvc(router =>
    {
       router.MapRoute("pet-delete", "/mypet/{petId}", defaults: new
       {
         controller = "MyPetApi",
         action = "DeletePet"
       });

       router.MapRoute("pet-getbyid", "/mypet/{petId}", defaults: new
       {
          controller = "MyPetApi",
          action = "GetPetById"
      });
      // more routes..
    
       ConventionalRoutingSwaggerGen.UseRoutes(router.Routes.ToList());
     });
    ```
## Compatibility ##

Currently `2.2.X` versions support only ASP.NET Core 2.2 applications, but support will be added for ASP.NET Core 3.X as well

## Notes ##
    
* Attribute based routes will continue to work as used to
* Conroller action methods are not required to have an `HTTP` method attribute. In case an action doesn't have an HTTP method attribute applied, the generator will assume it's a GET request
    
## License ##
Code released under the <a href="https://github.com/chsakell/Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting/blob/master/LICENSE" target="_blank"> MIT license</a>.
    
