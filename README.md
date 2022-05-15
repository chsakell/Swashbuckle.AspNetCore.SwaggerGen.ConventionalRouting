## Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting

Swagger generator extension with conventional based routes support.

NuGet: [Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting](https://www.nuget.org/packages/Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting/)

# Getting Started #

The package supports ASP.NET Core 2.2, 3.X, .NET 5.0 & .NET 6.0 applications

> Depending on your application's framework, follow the corresponding guide
<br/>

## ASP.NET Core 3.X, .NET 5.0, .NET 6.0 applications

1. Install the latest standard Nuget package into your ASP.NET Core application.

    ```
    Package Manager : Install-Package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
    CLI : dotnet add package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
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
    services.AddSwaggerGenWithConventionalRoutes(options =>
    {
        options.IgnoreTemplateFunc = (template) => template.StartsWith("api/");
        options.SkipDefaults = true;
    });
    ```
    
 3. In the `Configure` method of `Startup.cs`, add `Swagger` and `SwaggerUI` middlewares and after registering all your conventional routes, pass the `endpoints` as an argument to the Conventional Routing generator.
    
    ```csharp
    
    app.UseSwagger();

    app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
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
> In case you are not using endpoint routing, meaning you use the `.UseMvc` setup, configure the generator as follow:

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
  ConventionalRoutingSwaggerGen.UseRoutes(router.Routes);
 });
 ```

<br/> 

## ASP.NET Core 2.2 applications

1. Install the latest standard Nuget package into your ASP.NET Core application.

    ```
    Package Manager : Install-Package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
    CLI : dotnet add package Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
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
    services.AddSwaggerGenWithConventionalRoutes(options =>
    {
        options.IgnoreTemplateFunc = (template) => template.StartsWith("api/");
        options.SkipDefaults = true;
    });
    ```
    
 3. In the `Configure` method of `Startup.cs`, add `Swagger` and `SwaggerUI` middlewares and after registering all your conventional routes, pass them as an argument to the Conventional Routing generator.
    
    ```csharp
    
    app.UseSwagger();

    app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
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

<br/>

## Configuration ##

* Skip default _area_, _controller_ or _action_ values by setting `SwaggerRoutingOptions.SkipDefaults` = `true`. The resolver will skip the default values when possible.
```csharp
services.AddSwaggerGenWithConventionalRoutes(options =>
{
    options.SkipDefaults = true;
});
```
__Example__: For a `HomeController` having an `Index` action, the template will __/api/messages/__ for the following defined route: 
```
endpoints.MapControllerRoute("area-default", "api/messages/{controller=Home}/{action=Index}/{id?}");
```

* Ignore conventional routes based on their template by defining `SwaggerRoutingOptions` during registration. The following example will __ignore__ all routes having template that starts with `api/`

```csharp
services.AddSwaggerGenWithConventionalRoutes(options =>
{
    options.IgnoreTemplateFunc = (template) => template.StartsWith("api/");
});
```
> Keep in mind that the _template_ to be ignored is the exact template defined in the route configuration. This means that in the previous example `template.StartsWith("api/")` will not ignore routes that have been defined as `/api/other-segments/` which have a trailing slash at the beginning. Your `IgnoreTemplateFunc` is responsible to properly filter your routes.
* Ignore a specific conventional route by adding `[SwaggerConfig(IgnoreApi = true)]` on the controller action you wish to be ignore by swagger


## Compatibility ##

The package is compatible with ASP.NET Core 3.X and ASP.NET Core 2.2 applications.

|Generator Version|ASP.NET Core|Original dependency|
|----------|----------|----------|
|4.3.0|2.2, 3.X, 5.0, 6.0|6.3.1|
|4.1.0, 4.2.1|2.2, 3.X, 5.0|5.6.3|
|4.0.X|2.2, 3.X|5.6.3|
|3.0.1.X|3.X|5.5.1|
|2.2.1.X|2.2|5.5.1|

## Notes ##
    
* Attribute based routes will continue to work as used to
* The generator tries to match each controller action method to a registered route. It case multiple routes meet the action's descriptor definition, then the first route wins
* Controller action methods are not required to have an `HTTP` method attribute. The generator will assign a default HTTP method depending on the action's name as follow:

|Name starts with|Assigned HTTP method|
|----------|----------|
|Post|POST|
|Put, Create|PUT|
|Patch|PATCH|
|Delete, Remove|DELETE|
|all others|GET|

## Releases changes

Check the [CHANGELOG](https://github.com/chsakell/Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting/blob/master/CHANGELOG.md) for new releases and breaking changes between versions

## Contribution ##

* You are free to contribute by either improving the quality of the generator or opening an issue for a bug or feature request
* Master contains the implementations for all target frameworks so any contribution should cover them as well
* In case you open an issue, make sure you give enough details to reproduce it, for example if the generator creates a wrong swagger route _(returns 404)_ for a controller's action, then give the route configuration and the controller's action

## Donation ##
You can show your support to this project by making a donation via PayPal

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=SQK68T4HX9H56&currency_code=EUR&source=url)

## License ##
Code released under the <a href="https://github.com/chsakell/Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting/blob/master/LICENSE" target="_blank"> MIT license</a>
