using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Service;
using Valeting.Business;
using Valeting.ApiObjects;
using Valeting.Repositories;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class FlexibilityController : FlexibilityBaseController
    {
        private readonly IConfiguration _configuration;

        public FlexibilityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                FlexibilityService flexibilityService = new(new FlexibilityRepository(_configuration));
                FlexibilityDTO flexibility = await flexibilityService.FindByIDAsync(Guid.Parse(id));

                FlexibilityApi flexibilityApi = new()
                {
                    Id = flexibility.Id,
                    Description = flexibility.Description
                };

                return StatusCode((int)HttpStatusCode.OK, flexibilityApi);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> ListAllAsync()
        {
            try
            {
                List<FlexibilityApi> flexibilityApis = new();

                FlexibilityService flexibilityService = new(new FlexibilityRepository(_configuration));
                IEnumerable<FlexibilityDTO> flexibilities = await flexibilityService.ListAllAsync();

                flexibilityApis.AddRange(
                    flexibilities.Select(item => new FlexibilityApi()
                    {
                        Id = item.Id,
                        Description = item.Description
                    })
                );

                return StatusCode((int)HttpStatusCode.OK, flexibilityApis);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
