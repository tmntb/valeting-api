using System.ComponentModel.DataAnnotations;

namespace Valeting.SwaggerDocumentation.Parameter
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PathParameterAttribute : ValidationAttribute
    {
        public string Description { get; set; }
        public string Example { get; set; }
        public string Format { get; set; }

        public PathParameterAttribute(string description, string example, string format)
        {
            Description = description;
            Example = example;
            Format = format;
        }
    }
}
