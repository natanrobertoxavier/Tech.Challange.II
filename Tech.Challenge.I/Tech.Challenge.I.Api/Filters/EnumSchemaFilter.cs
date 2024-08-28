using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace Tech.Challenge.I.Api.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var enumType = context.Type;
            schema.Type = "string";
            schema.Format = null;
            schema.Enum.Clear();

            var enumNames = Enum.GetNames(enumType);

            foreach (var enumName in enumNames)
            {
                var enumMember = enumType.GetMember(enumName).First();
                var descriptionAttribute = enumMember.GetCustomAttribute<DescriptionAttribute>();
                var description = descriptionAttribute?.Description ?? enumName;
                schema.Enum.Add(new OpenApiString(description));
            }
        }
    }
}