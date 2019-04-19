using System;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyCompanyName.AbpZeroTemplate.Web.Startup.Swagger
{
    public class SwaggerEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            var type = Nullable.GetUnderlyingType(context.SystemType) ?? context.SystemType;
            if (!type.IsEnum)
            {
                return;
            }

            if (!context.SchemaRegistry.Definitions.ContainsKey(type.Name) || context.SchemaRegistry.Definitions[type.Name] != null)
            {
                schema.Ref = context.SchemaRegistry.GetOrRegister(type).Ref;
            }
            else
            {
                schema.Extensions.Add("x-enumNames", Enum.GetNames(type));
            }
        }
    }
}