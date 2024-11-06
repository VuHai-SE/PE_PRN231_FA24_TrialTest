using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PE_PRN231_FA24_TrialTest_2_FE.Dto;

namespace PE_PRN231_FA24_TrialTest_2_FE.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7251/api/auth/login", new { email = Email, password = Password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                HttpContext.Session.SetString("UserRole", result.User.Role);
                HttpContext.Session.SetString("Token", result.Token);
                HttpContext.Session.SetString("Email", result.User.Email);

                // Redirect based on role if necessary
                if (result.User.Role == "doctor")
                {
                    return RedirectToPage("/Doctor/Index");
                }
                else if (result.User.Role == "admin")
                {
                    return RedirectToPage("/Admin/Index");
                }
                else if (result.User.Role == "patient")
                {
                    return RedirectToPage("/Patient/Index", new { id = result.User.Id });
                }
            }

            ErrorMessage = "You are not allowed to access this function!";
            return Page();
        }
    }
}
