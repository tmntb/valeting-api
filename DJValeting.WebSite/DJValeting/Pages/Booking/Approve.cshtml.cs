using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using DJValeting.Data;

namespace DJValeting.Pages.Booking
{
    public class ApproveModel : PageModel
    {
        private readonly HttpClient _httpClient = new();
        private IConfiguration _configuration;
        private DJValetingContext _djValetingContext;
        public Models.Booking Booking;

        public ApproveModel(DJValetingContext context, IConfiguration configuration)
        {
            _djValetingContext = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            if (DJValetingContext.Login)
            {
                Booking = _djValetingContext.Bookings.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
                Booking.Approved = true;

                var json = JsonConvert.SerializeObject(Booking);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                string endpoint = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "bookings");
                await _httpClient.PutAsync(string.Format("{0}/{1}", endpoint, Booking.Id), data);

                _djValetingContext.Refresh = true;

                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("./Create");
            }
        }
    }
}
