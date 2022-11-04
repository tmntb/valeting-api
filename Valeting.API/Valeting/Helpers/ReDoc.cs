using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Builder;

namespace Valeting.Helpers
{
    public static class ReDoc
    {
        static readonly string API_CONTRACT_FILE = "../contract/valeting_v1.yml"; //arranjar forma de ir buscar sempre a última versão (bd?, string.format?)
        static readonly string API_TITLE = "Valeting";

        public static void UseReDocConfig(this IApplicationBuilder app)
        {
            app.UseReDoc(options =>
            {
                options.DocumentTitle = API_TITLE;
                options.SpecUrl = API_CONTRACT_FILE;
            });
        }

        public static void EnableStaticFiles(this IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".yml"] = "text/yml";
            provider.Mappings[".map"] = "application/json";
            provider.Mappings[".js.map"] = "application/json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }
    }
}

