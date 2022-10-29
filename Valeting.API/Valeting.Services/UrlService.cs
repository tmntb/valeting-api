using System.Web;

using Valeting.Business;
using Valeting.Services.Interfaces;

namespace Valeting.Services
{
    public class UrlService : IUrlService
    {
        public string GenerateSelf(string baseUrl, string path)
        {
            return string.Format("https://{0}{1}", baseUrl, path);
        }

        public string GenerateSelf(string baseUrl, string path, Guid id)
        {
            return string.Format("https://{0}{1}/{2}", baseUrl, path, id);
        }

        public LinkDTO GeneratePaginatedLinks(string baseUrl, string path, string queryString, int pageNumber, int totalPages, object filter)
        {
            var linkDTO = new LinkDTO()
            {
                Prev = string.Empty,
                Next = string.Empty,
                Self = string.Empty
            };

            var queryStringStr = string.Empty;

            if (pageNumber > 1)
            {
                var pg = filter.GetType().GetProperty("PageNumber");
                pg.SetValue(filter, pageNumber - 1);
                queryStringStr = BuildQueryString(filter);

                linkDTO.Prev = string.Format("https://{0}{1}?{2}", baseUrl, path, queryStringStr);
            }

            if (pageNumber < totalPages)
            {
                var pg = filter.GetType().GetProperty("PageNumber");
                pg.SetValue(filter, pageNumber + 1);
                queryStringStr = BuildQueryString(filter);

                linkDTO.Next = string.Format("https://{0}{1}?{2}", baseUrl, path, queryStringStr);
            }

            linkDTO.Self = string.Format("https://{0}{1}{2}", baseUrl, path, queryString);

            return linkDTO;
        }

        private string BuildQueryString(object filter)
        {
            var properties = from p in filter.GetType().GetProperties().OrderBy(y => y.CustomAttributes.FirstOrDefault().NamedArguments.FirstOrDefault(i => i.MemberName.Equals("Order")).TypedValue.Value)
                             where p.GetValue(filter, null) != null
                             select p.CustomAttributes.FirstOrDefault().NamedArguments.FirstOrDefault().TypedValue.Value + "=" + HttpUtility.UrlEncode(p.GetValue(filter, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}

