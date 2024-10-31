using Domain.Entities;
using Domain.Interfaces.Services;
using Dto;
using Dto.Requests;
using Dto.Responses;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service;
using System.Text.RegularExpressions;

namespace PE_PRN231_FA24_TrialTest_2_BE.Controllers
{
    [Route("api/person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IVirusService _virusService;

        public PersonController(IPersonService personService, IVirusService virusService)
        {
            _personService = personService;
            _virusService = virusService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPersonWithViruses([FromBody] AddPersonRequest request)
        {
            // Validate Fullname format
            if (!IsValidFullname(request.FullName))
            {
                return BadRequest(new { error = "Each word of the Fullname must begin with the capital letter and can contain a-z, A-Z, space, @, #, and digits 0-9." });
            }

            // Validate Birthday
            if (request.BirthDay >= new DateTime(2007, 1, 1))
            {
                return BadRequest(new { error = "Value for Birthday must be earlier than 01-01-2007." });
            }

            // Validate Phone Number format
            if (!IsValidPhoneNumber(request.Phone))
            {
                return BadRequest(new { error = "Phone number must be in the format +84989xxxxxx." });
            }

            // Validate Resistance Rate for each virus
            foreach (var virusInfo in request.Viruses)
            {
                if (virusInfo.ResistanceRate < 0 || virusInfo.ResistanceRate > 1)
                {
                    return BadRequest(new { error = "Resistance Rate must be between 0 and 1." });
                }
            }

            var person = new Person
            {
                PersonId = request.PersonId,
                Fullname = request.FullName,
                BirthDay = DateOnly.FromDateTime(request.BirthDay),
                Phone = request.Phone
            };

            // Fetch or create virus IDs using VirusService
            var personViruses = new List<PersonVirus>();
            foreach (var virusInfo in request.Viruses)
            {
                var virusId = await _virusService.GetOrCreateVirusIdByNameAsync(virusInfo.VirusName);
                personViruses.Add(new PersonVirus
                {
                    PersonId = person.PersonId,
                    VirusId = virusId,
                    ResistanceRate = virusInfo.ResistanceRate
                });
            }

            await _personService.AddPersonWithVirusesAsync(person, personViruses);

            return CreatedAtAction(nameof(GetPersonDetails), new { id = person.PersonId }, new AddPersonResponse
            {
                PersonId = person.PersonId,
                Message = "Person and viruses added successfully"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonDetails(int id)
        {
            var person = await _personService.GetPersonDetailsAsync(id);

            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            var response = new GetPersonResponse
            {
                PersonId = person.PersonId,
                FullName = person.Fullname,
                BirthDay = person.BirthDay.ToDateTime(new TimeOnly(0, 0)),
                Phone = person.Phone,
                Viruses = person.PersonViruses.Select(pv => new VirusInfo
                {
                    VirusName = pv.Virus.VirusName,
                    ResistanceRate = (float)pv.ResistanceRate
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet("persons")]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _personService.GetAllPersonsAsync();
            var response = persons.Select(person => new GetPersonResponse
            {
                PersonId = person.PersonId,
                FullName = person.Fullname,
                BirthDay = person.BirthDay.ToDateTime(new TimeOnly(0, 0)),
                Phone = person.Phone,
                Viruses = person.PersonViruses.Select(pv => new VirusInfo
                {
                    VirusName = pv.Virus.VirusName,
                    ResistanceRate = (float)pv.ResistanceRate
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] UpdatePersonRequest request)
        {
            // The same validation logic can be reused here as in the AddPersonWithViruses method.

            if (!IsValidFullname(request.FullName))
            {
                return BadRequest(new { error = "Each word of the Fullname must begin with the capital letter and can contain a-z, A-Z, space, @, #, and digits 0-9." });
            }

            if (request.BirthDay >= new DateTime(2007, 1, 1))
            {
                return BadRequest(new { error = "Value for Birthday must be earlier than 01-01-2007." });
            }

            if (!IsValidPhoneNumber(request.Phone))
            {
                return BadRequest(new { error = "Phone number must be in the format +84xxxxxxxxx." });
            }

            foreach (var virusInfo in request.Viruses)
            {
                if (virusInfo.ResistanceRate < 0 || virusInfo.ResistanceRate > 1)
                {
                    return BadRequest(new { error = "Resistance Rate must be between 0 and 1." });
                }
            }

            var person = new Person
            {
                PersonId = id,
                Fullname = request.FullName,
                BirthDay = DateOnly.FromDateTime(request.BirthDay),
                Phone = request.Phone
            };

            var personViruses = new List<PersonVirus>();
            foreach (var virusInfo in request.Viruses)
            {
                var virusId = await _virusService.GetOrCreateVirusIdByNameAsync(virusInfo.VirusName);
                personViruses.Add(new PersonVirus
                {
                    PersonId = person.PersonId,
                    VirusId = virusId,
                    ResistanceRate = virusInfo.ResistanceRate
                });
            }

            await _personService.UpdatePersonAsync(person, personViruses);

            return Ok(new { message = "Person and viruses updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok(new { message = "Person and related viruses deleted successfully" });
        }

        // Validation methods
        private bool IsValidFullname(string fullName)
        {
            // Regex pattern to validate full name with each word starting with a capital letter and allowing only specified characters
            var fullNamePattern = @"^([A-Z][a-z0-9@#]*\s)*[A-Z][a-z0-9@#]*$";
            return Regex.IsMatch(fullName, fullNamePattern);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Pattern to match phone numbers in international format like +84989xxxxxx
            var phonePattern = @"^\+84\d{9}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }
    }
}
