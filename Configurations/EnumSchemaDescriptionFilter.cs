using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BetRoyale.API.Configurations;

public class EnumSchemaDescriptionFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var enumType = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!enumType.IsEnum)
        {
            return;
        }

        var enumValues = Enum.GetValues(enumType)
            .Cast<object>()
            .Select(value => $"{Convert.ToInt32(value)} = {Enum.GetName(enumType, value)}")
            .ToArray();

        var enumDescription = $"Allowed values: {string.Join(", ", enumValues)}.";
        schema.Description = string.IsNullOrWhiteSpace(schema.Description)
            ? enumDescription
            : $"{schema.Description} {enumDescription}";

        if (schema.Example is null)
        {
            var firstValue = Enum.GetValues(enumType).GetValue(0);
            if (firstValue is not null)
            {
                schema.Example = new OpenApiInteger(Convert.ToInt32(firstValue));
            }
        }
    }
}
