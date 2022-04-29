using DJValeting.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DJValeting.Pages.Booking
{
    public class IndexModel : PageModel
    {
        private DJValetingContext _djValetingContext;
        public List<Models.Booking> Bookings { get; set; }

        public IndexModel(DJValetingContext djValetingContext)
        {
            _djValetingContext = djValetingContext;
        }

        public IActionResult OnGet()
        {
            if (DJValetingContext.Login)
            {
                _djValetingContext.Refresh = true;
                Bookings = _djValetingContext.Bookings;

                return Page();
            }
            else
            {
                return RedirectToPage("./Create");
            }
        }
    }
}
