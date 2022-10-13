using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Controllers.BaseController;
using Valeting.Services.Interfaces;

namespace Valeting.Controllers
{
    public class FlexibilityController : FlexibilityBaseController
    {
        private readonly IFlexibilityService _flexibilityService;

        public FlexibilityController(IFlexibilityService flexibilityService)
        {
            _flexibilityService = flexibilityService;
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                var flexibility = await _flexibilityService.FindByIDAsync(Guid.Parse(id));

                var flexibilityApi = new FlexibilityApi()
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
                var flexibilityApis = new List<FlexibilityApi>();

                var flexibilities = await _flexibilityService.ListAllAsync();

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
