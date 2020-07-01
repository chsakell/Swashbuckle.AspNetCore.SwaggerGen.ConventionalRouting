using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;

namespace PetStore.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var descriptor_1 =
                new ServiceDescriptor(
                    typeof(IApiDescriptionProvider),
                    typeof(ConventionalRoutingApiDescriptionProvider),
                    ServiceLifetime.Transient);

            var descriptor_3 =
                new ServiceDescriptor(
                    typeof(IActionDescriptorCollectionProvider),
                    typeof(ConventionalRoutingActionDescriptorCollectionProvider),
                    ServiceLifetime.Transient);

            services.Replace(descriptor_1);
            services.Replace(descriptor_3);
            services.AddSingleton<IRouteTemplateResolver, RouteTemplateResolver>();

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
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

                router.MapRoute("pet-find-by-status", "/mypet/findByStatus", defaults: new
                {
                    controller = "MyPetApi",
                    action = "FindPetsByStatus"
                });

                router.MapRoute("pet-upload-image", "/mypet/{petId}/uploadImage", defaults: new
                {
                    controller = "MyPetApi",
                    action = "UploadFile"
                });

                router.MapRoute("pet-find-by-tags", "/mypet/findByTags", defaults: new
                {
                    controller = "MyPetApi",
                    action = "FindPetsByTags"
                });

                // regex test
                router.MapRoute(
                    name: "hello-world-regex",
                    template: "message/{controller:regex(^H.*)=HelloWorld}/{action:regex(^Index$|^About$)=Index}/{message:alpha?}");

                router.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                ConventionalRoutingSwaggerGenMiddleware.UseRoutes(router.Routes.ToList());
            });
        }
    }
}
