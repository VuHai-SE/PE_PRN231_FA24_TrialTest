using Domain.Entities;
using Domain.Interfaces.Services;
using Dto;
using Dto.Requests;
using Dto.Responses;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service;

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
            var person = new Person
            {
                PersonId = id,
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

            await _personService.UpdatePersonAsync(person, personViruses);

            return Ok(new { message = "Person and viruses updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            await _personService.DeletePersonAsync(id);
            return Ok(new { message = "Person and related viruses deleted successfully" });
        }
    }
}
