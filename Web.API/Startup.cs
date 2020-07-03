using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddSwaggerGen();
            services.AddSwaggerGenWithConventionalRoutes();
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

                router.MapRoute("hello-friend-test", "/test/{id:int}/friend/{message:alpha}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test"
                });

                router.MapRoute("hello-friend-test-date", "/test/{date:datetime}/friend/{message:alpha}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test2"
                });

                router.MapRoute("hello-friend-test3", "/test/{num1:decimal}/friend/{num2:long}/{token:guid}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test3"
                });

                router.MapRoute("sport-event-details", "{sport}/{event}/{eventId:int}", new
                {
                    controller = "Sport",
                    action = "Details"
                }, new
                {
                    sport = new GenericMatchRouteConstraint(new[] { "football", "volley" })
                });

                router.MapRoute("sport-get-events", "sport/events/{eventIds}", new
                {
                    controller = "Sport",
                    action = "GetEvents"
                });

                router.MapRoute("sport-live-events", "livesports", new
                {
                    controller = "Sport",
                    action = "Live"
                });

                router.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                ConventionalRoutingSwaggerGen.UseRoutes(router.Routes.ToList());
            });
        }
    }
}
