using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DJValeting.Data;

namespace DJValeting.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            DJValetingContext.User = null;
            DJValetingContext.Login = false;

            return RedirectToPage("../Booking/Create");
        }
    }
}
