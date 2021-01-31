using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting;

namespace Web.API
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
            services.AddControllers();

            services.AddSwaggerGen();
            services.AddSwaggerGenWithConventionalRoutes(options =>
            {
                options.IgnoreTemplateFunc = (template) => template.StartsWith("api/");
                options.SkipDefaults = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthorization();

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

                endpoints.MapControllerRoute("pet-find-by-status", "/mypet/findByStatus", defaults: new
                {
                    controller = "MyPetApi",
                    action = "FindPetsByStatus"
                });

                endpoints.MapControllerRoute("pet-upload-image", "/mypet/{petId}/uploadImage", defaults: new
                {
                    controller = "MyPetApi",
                    action = "UploadFile"
                });

                endpoints.MapControllerRoute("pet-find-by-tags", "/mypet/findByTags", defaults: new
                {
                    controller = "MyPetApi",
                    action = "FindPetsByTags"
                });

                // regex test
                endpoints.MapControllerRoute(
                    name: "hello-world-regex",
                    pattern: "message/{controller:regex(^H.*)=HelloWorld}/{action:regex(^Index$|^About$)=Index}/{message:alpha?}");

                endpoints.MapControllerRoute("hello-friend-test", "/test/{id:int}/friend/{message:alpha}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test"
                });

                endpoints.MapControllerRoute("hello-friend-test-date", "/test/{date:datetime}/friend/{message:alpha}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test2"
                });

                endpoints.MapControllerRoute("hello-friend-test3", "/test/{num1:decimal}/friend/{num2:long}/{token:guid}", defaults: new
                {
                    controller = "HelloFriend",
                    action = "Test3"
                });

                endpoints.MapControllerRoute("sport-event-details", "{sport}/{event}/{eventId:int}", new
                {
                    controller = "Sport",
                    action = "Details"
                }, new
                {
                    sport = new GenericMatchRouteConstraint(new[] { "football", "volley" })
                });

                endpoints.MapControllerRoute("sport-get-events", "sport/events/{eventIds}", new
                {
                    controller = "Sport",
                    action = "GetEvents"
                });

                endpoints.MapControllerRoute("sport-live-events", "livesports", new
                {
                    controller = "Sport",
                    action = "Live"
                });

                endpoints.MapControllerRoute("sport-event-details", "{oddspath}/{teams}/{id}", new
                {
                    controller = "Sport",
                    action = "EventDetails"
                }, new
                {
                    id = @"\d+",
                    oddspath = new GenericMatchRouteConstraint(new[] { "odds", "matchodds" })
                });


                endpoints.MapControllerRoute("area-default", "api/{area}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                ConventionalRoutingSwaggerGen.UseRoutes(endpoints);
            });
        }
    }
}
