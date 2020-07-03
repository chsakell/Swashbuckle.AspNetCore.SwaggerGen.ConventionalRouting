using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public interface IConventionalRoutingSwaggerProvider : ISwaggerProvider
    {

    }

    public class ConventionalRoutingSwaggerGenerator : SwaggerGenerator, IConventionalRoutingSwaggerProvider
    {
        public ConventionalRoutingSwaggerGenerator(SwaggerGeneratorOptions options, 
            IConventionalRoutingApiDescriptionGroupCollectionProvider apiDescriptionsProvider, 
            ISchemaGenerator schemaGenerator) : base(options, apiDescriptionsProvider, schemaGenerator)
        {
            
        }
    }
}
