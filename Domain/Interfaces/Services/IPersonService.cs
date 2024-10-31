using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IPersonService
    {
        Task<IActionResult> AddPersonWithViruses(AddPersonRequest request);
        Task<IActionResult> GetPersonDetails(int id);
        Task<IActionResult> GetAllPersons();
        Task<IActionResult> UpdatePerson(int id, UpdatePersonRequest request);
        Task<IActionResult> DeletePerson(int id);
    }
}
