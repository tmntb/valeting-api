using DJValeting.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DJValeting.Pages.Booking
{
    public class DeleteModel : PageModel
    {
        private DJValetingContext _djValetingContext;
        private IConfiguration _configuration;
        private readonly HttpClient _httpClient = new();
        public Models.Booking Booking { get; set; }

        public DeleteModel(DJValetingContext djValetingContext, IConfiguration configuration)
        {
            _djValetingContext = djValetingContext;
            _configuration = configuration;
        }

        public IActionResult OnGet(string id)
        {
            if (DJValetingContext.Login)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();

                Booking = _djValetingContext.Bookings.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));

                return Page();
            }
            else
            {
                return RedirectToPage("./Create");
            }
        }

        public async Task<IActionResult> OnPost(string id)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var endpoint = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "bookings");
            await _httpClient.DeleteAsync(string.Format("{0}/{1}", endpoint, id));

            _djValetingContext.Refresh = true;

            return RedirectToPage("./Index");
        }
    }
}
