using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PE_PRN231_FA24_TrialTest_2_FE.Dto;

namespace PE_PRN231_FA24_TrialTest_2_FE.Pages.Patient
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public GetPersonResponse Patient { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7251/api/person/{id}");

                if (response.IsSuccessStatusCode)
                {
                    Patient = await response.Content.ReadFromJsonAsync<GetPersonResponse>();
                    if (Patient == null)
                    {
                        ErrorMessage = "Patient data could not be found.";
                        return Page();
                    }
                }
                else
                {
                    ErrorMessage = $"Error fetching data: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Request error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }

            return Page();
        }
    }
}
