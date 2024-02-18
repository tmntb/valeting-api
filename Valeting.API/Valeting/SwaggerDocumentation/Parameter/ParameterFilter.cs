using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Valeting.SwaggerDocumentation.Parameter
{
    public class ParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (context.PropertyInfo != null)
            {
                var attributes = context.PropertyInfo.GetCustomAttributes(true);
                IEnumerable<QueryParameterAttribute>? queryParameterAttributes = attributes.OfType<QueryParameterAttribute>();
                if (queryParameterAttributes != null && queryParameterAttributes.Any())
                    AddExample(parameter, queryParameterAttributes);
            }

            if (context.ParameterInfo != null)
            {
                var attributes = context.ParameterInfo.GetCustomAttributes(true);
                IEnumerable<PathParameterAttribute>? pathParameterAttributes = attributes.OfType<PathParameterAttribute>();
                if (pathParameterAttributes != null && pathParameterAttributes.Any())
                    AddExample(parameter, pathParameterAttributes);
            }
        }

        private void AddExample(OpenApiParameter parameter, IEnumerable<QueryParameterAttribute> parameterAttributes)
        {
            foreach (var item in parameterAttributes)
            {
                parameter.Description = item.Description;
                parameter.Schema.Example = new OpenApiString(item.Example);
                parameter.Schema.Minimum = item.Minimum;
                if(item.Maximum != 0)
                    parameter.Schema.Maximum = item.Maximum;
            }
        }

        private void AddExample(OpenApiParameter parameter, IEnumerable<PathParameterAttribute> parameterAttributes)
        {
            foreach (var item in parameterAttributes)
            {
                parameter.Description = item.Description;
                parameter.Schema.Example = new OpenApiString(item.Example);
                parameter.Schema.Format = item.Format;
            }
        }
    }
}
