using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using DJValeting.Data;
using DJValeting.Models;

namespace DJValeting.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient = new();
        private IConfiguration _configuration;
        [BindProperty]
        public User User { get; set; }

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string email = User.Username;
            string password = User.Password;


            var json = JsonConvert.SerializeObject(User);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = string.Format("{0}{1}", _configuration.GetSection("EndpointDJValeting").Value, "users/verify");
            var response = await _httpClient.PostAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                string resultLogin = await response.Content.ReadAsStringAsync();
                DJValetingContext.Login = JsonConvert.DeserializeObject<bool>(resultLogin);

                User.Username = email;
                User.Password = password;
                DJValetingContext.User = User;
            }

            return RedirectToPage("../Booking/Index");
        }
    }
}
