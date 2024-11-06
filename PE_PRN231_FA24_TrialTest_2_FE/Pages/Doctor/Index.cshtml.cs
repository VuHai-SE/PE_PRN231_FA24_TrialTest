using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PE_PRN231_FA24_TrialTest_2_FE.Dto;

namespace PE_PRN231_FA24_TrialTest_2_FE.Pages.Doctor
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<GetPersonResponse> Patients { get; set; } = new List<GetPersonResponse>();
        public string ErrorMessage { get; set; }

        [BindProperty]
        public AddPersonRequest AddPatient { get; set; } = new AddPersonRequest
        {
            Viruses = new List<VirusInfo> { new VirusInfo() } // Initialize with one VirusInfo
        };

        public async Task OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "doctor")
            {
                ErrorMessage = "You do not have permission to view this page.";
                return;
            }

            var response = await _httpClient.GetAsync("https://localhost:7251/api/person/persons");
            if (response.IsSuccessStatusCode)
            {
                Patients = await response.Content.ReadFromJsonAsync<List<GetPersonResponse>>();
            }
            else
            {
                ErrorMessage = "Failed to load patient data.";
            }
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields correctly.";
                return Page();
            }

            var addRequest = new AddPersonRequest
            {
                FullName = AddPatient.FullName,
                BirthDay = AddPatient.BirthDay,
                Phone = AddPatient.Phone,
                Viruses = AddPatient.Viruses // Use the populated list of viruses from the form
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7251/api/person", addRequest);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(); // Refresh the page to show the updated list
            }

            ErrorMessage = "Failed to add new patient.";
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7251/api/person/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(); // Refresh the page to show the updated list
            }

            ErrorMessage = "Failed to delete patient.";
            return Page();
        }
    }
}
