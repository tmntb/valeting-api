using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using DJValeting.Data;
using DJValeting.Models;

namespace DJValeting.Pages.Booking
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient = new();
        private IConfiguration _configuration;
        private DJValetingContext _djValetingContext;
        [BindProperty]
        public Models.Booking Booking { get; set; }
        public List<Flexibility> Flexibility { get; set; }
        public List<VehicleSize> VehicleSize { get; set; }

        public EditModel(DJValetingContext djValetingContext, IConfiguration configuration)
        {
            _djValetingContext = djValetingContext;
            _configuration = configuration;
        }

        public IActionResult OnGet(string id)
        {
            if (DJValetingContext.Login)
            {
                Flexibility = _djValetingContext.Flexibilities;
                VehicleSize = _djValetingContext.VehicleSizes;
                Booking = _djValetingContext.Bookings.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));

                return Page();
            }
            else
            {
                return RedirectToPage("./Create");
            }
        }

        public async Task<IActionResult> OnPost(IFormCollection collection)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            var booking = new Models.Booking()
            {
                Name = Booking.Name,
                BookingDate = Booking.BookingDate,
                Flexibility = new Flexibility() { Id = Guid.Parse(collection["FlexibilityId"]) },
                VehicleSize = new VehicleSize() { Id = Guid.Parse(collection["VehicleSizeId"]) },
                ContactNumber = Booking.ContactNumber,
                Email = Booking.Email,
                Approved = Booking.Approved
            };

            var json = JsonConvert.SerializeObject(booking);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            string endpoint = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "bookings");
            await _httpClient.PutAsync(string.Format("{0}/{1}", endpoint, Booking.Id), data);

            _djValetingContext.Refresh = true;

            return RedirectToPage("./Index");
        }
    }
}
