using System.Text;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DJValeting.Data;
using DJValeting.Models;

namespace DJValeting.Pages.Booking
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient = new();
        private IConfiguration _configuration;
        private DJValetingContext _djValetingContext;
        [BindProperty]
        public Models.Booking Booking { get; set; }
        public List<Flexibility> Flexibility { get; set; }
        public List<VehicleSize> VehicleSize { get; set; }

        public CreateModel(DJValetingContext djValetingContext, IConfiguration configuration)
        {
            _djValetingContext = djValetingContext;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            Flexibility = _djValetingContext.Flexibilities;
            VehicleSize = _djValetingContext.VehicleSizes;

            return Page();
        }

        public async Task<IActionResult> OnPost(IFormCollection collection)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var booking = new Models.Booking
            {
                Name = Booking.Name,
                BookingDate = Booking.BookingDate,
                Flexibility = new Flexibility() { Id = Guid.Parse(collection["FlexibilityId"]) },
                VehicleSize = new VehicleSize() { Id = Guid.Parse(collection["VehicleSizeId"]) },
                ContactNumber = Booking.ContactNumber,
                Email = Booking.Email
            };

            var json = JsonConvert.SerializeObject(booking);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "bookings");
            await _httpClient.PostAsync(endpoint, data);

            _djValetingContext.Refresh = true;

            if (DJValetingContext.Login)
                return RedirectToPage("./Index");
            else
                return RedirectToPage("./Create");
        }
    }
}
