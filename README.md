## Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting

Swagger generator extension for conventional based routes support.

# Getting Started #

There are 2 versions of the package, one for ASP.NET Core 2.2 applications and another one for ASP.NET Core 3.X.
Follow the guide you are interesting in.

## ASP.NET Core 2.2 applications

1. Install the standard Nuget package into your ASP.NET Core application.

    ```
    Package Manager : Install-Package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting -Version 2.2.1.1
    CLI : dotnet add package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting --version 2.2.1.1
    ```
> The extension has a dependency on the [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) so you don't have to install it twice.


2. In the `ConfigureServices` method of `Startup.cs`, register the [default](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen) Swagger generator and then the Conventional Routing Swagger generator.
    ```csharp
    using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;
    ```
    
    ```csharp
    services.AddMvc();
    services.AddSwaggerGen();
    
    // this will handle all conventional based routes
    services.AddSwaggerGenWithConventionalRoutes();
    ```
    
 3. In the `Configure` method of `Startup.cs`, after registering all your conventional routes, pass them as an argument to the Conventional Routing generator.
    
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
      
      // routes with common or custom constraints will work
      router.MapRoute(name: "hello-world-regex",
           template: "message/{controller:regex(^H.*)=HelloWorld}/{action:regex(^Index$|^About$)=Index}/{message:alpha?}");
           
      // more routes..
      
      // Pass the conventional routes to the generator
      ConventionalRoutingSwaggerGen.UseRoutes(router.Routes.ToList());
     });
    ```

## ASP.NET Core 3.X applications

1. Install the standard Nuget package into your ASP.NET Core application.

    ```
    Package Manager : Install-Package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting -Version 3.0.1.1
    CLI : dotnet add package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting --version 3.0.1.1
    ```
> The extension has a dependency on the [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) so you don't have to install it twice.


2. In the `ConfigureServices` method of `Startup.cs`, register the [default](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen) Swagger generator and then the Conventional Routing Swagger generator.
    ```csharp
    using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;
    ```
    
    ```csharp
    services.AddControllers();
    services.AddSwaggerGen();
    
    // this will handle all conventional based routes
    services.AddSwaggerGenWithConventionalRoutes();
    ```
    
 3. In the `Configure` method of `Startup.cs`, after registering all your conventional routes, pass the `endpoints` as an argument to the Conventional Routing generator.
    
    ```csharp
    app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute("pet-delete", "/mypet/{petId}", defaults: new
                {
                    controller = "MyPetApi",
                    action = "DeletePet"
                });

                endpoints.MapControllerRoute("pet-getbyid", "/mypet/{petId}", defaults: new
                {
                    controller = "MyPetApi",
                    action = "GetPetById"
                });
                
                endpoints.MapControllerRoute(
                    name: "hello-world-regex",
                    pattern: "message/{controller:regex(^H.*)=HelloWorld}/{action:regex(^Index$|^About$)=Index}/{message:alpha?}");

      
               // routes with common or custom constraints will work
                endpoints.MapControllerRoute("sport-event-details", "{sport}/{event}/{eventId:int}", new
                {
                    controller = "Sport",
                    action = "Details"
                }, new
                {
                    sport = new GenericMatchRouteConstraint(new[] { "football", "volley" })
                });
           
       // more routes..
       
       // Pass the conventional routes to the generator
       ConventionalRoutingSwaggerGen.UseRoutes(endpoints);
     });
    ```
   

## Compatibility ##

Currently `2.2.X` versions support only ASP.NET Core 2.2 applications, but support will be added for ASP.NET Core 3.X as well.

|Generator Version|ASP.NET Core|Original dependency|
|----------|----------|----------|
|3.0.1.1|3.X|5.5.1|
|2.2.1.1|2.2|5.5.1|

## Notes ##
    
* Attribute based routes will continue to work as used to
* The generator tries to match each controller action method to a registered route. It case multiple routes meet the action's descriptor definition, then the first route wins
* Controller action methods are not required to have an `HTTP` method attribute. In case an action doesn't have an HTTP method attribute applied, the generator will assume it's a GET request

## Roadmap ##

* Add configuration to change the generator's behavior
* Add unit tests

## Contribution ##

You are free to contribute by either improving the quality of the generator or opening an issue for a bug or feature request
    
## License ##
Code released under the <a href="https://github.com/chsakell/Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting/blob/master/LICENSE" target="_blank"> MIT license</a>
