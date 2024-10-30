using Domain.Entities;
using Dto.Requests;
using Dto.Responses;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PE_PRN231_FA24_TrialTest_2_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ViroCureFal2024dbContext _context;

        public PersonController(ViroCureFal2024dbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPersonWithViruses([FromBody] AddPersonRequest request)
        {
            // Kiểm tra xem người đã tồn tại chưa
            var existingPerson = await _context.People.FindAsync(request.PersonId);
            if (existingPerson != null)
            {
                return Conflict(new { message = "Person with the same ID already exists." });
            }

            // Tạo một đối tượng Person
            var person = new Person
            {
                PersonId = request.PersonId,
                Fullname = request.FullName,
                BirthDay = DateOnly.FromDateTime(request.BirthDay),
                Phone = request.Phone
            };

            // Thêm Person vào context
            _context.People.Add(person);

            // Thêm các virus vào context
            foreach (var virusInfo in request.Viruses)
            {
                // Tìm virus trong hệ thống hoặc thêm mới nếu chưa tồn tại
                var virus = await _context.Viruses.FirstOrDefaultAsync(v => v.VirusName == virusInfo.VirusName);
                if (virus == null)
                {
                    virus = new Virus
                    {
                        VirusName = virusInfo.VirusName,
                        Treatment = null // Giá trị Treatment có thể để trống nếu chưa có
                    };
                    _context.Viruses.Add(virus);
                    await _context.SaveChangesAsync(); // Lưu thay đổi để có `virus_id`
                }

                // Tạo đối tượng PersonVirus
                var personVirus = new PersonVirus
                {
                    PersonId = person.PersonId,
                    VirusId = virus.VirusId,
                    ResistanceRate = virusInfo.ResistanceRate
                };

                _context.PersonViruses.Add(personVirus);
            }

            // Lưu tất cả thay đổi vào database
            await _context.SaveChangesAsync();

            // Tạo phản hồi
            var response = new AddPersonResponse
            {
                PersonId = person.PersonId,
                Message = "Person and viruses added successfully"
            };

            return CreatedAtAction(nameof(AddPersonWithViruses), new { id = person.PersonId }, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonDetails(int id)
        {
            var person = await _context.People
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .FirstOrDefaultAsync(p => p.PersonId == id);

            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            var response = new GetPersonResponse
            {
                PersonId = person.PersonId,
                FullName = person.Fullname,
                BirthDay = person.BirthDay.ToDateTime(new TimeOnly(0, 0)), // Chuyển đổi DateOnly sang DateTime
                Phone = person.Phone,
                Viruses = person.PersonViruses.Select(pv => new Dto.Responses.VirusInfo
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
            var persons = await _context.People
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .ToListAsync();

            var response = persons.Select(person => new GetPersonResponse
            {
                PersonId = person.PersonId,
                FullName = person.Fullname,
                BirthDay = person.BirthDay.ToDateTime(new TimeOnly(0, 0)), // Chuyển đổi DateOnly sang DateTime
                Phone = person.Phone,
                Viruses = person.PersonViruses.Select(pv => new Dto.Responses.VirusInfo
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
            var person = await _context.People
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .FirstOrDefaultAsync(p => p.PersonId == id);

            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            // Cập nhật thông tin của Person
            person.Fullname = request.FullName;
            person.BirthDay = DateOnly.FromDateTime(request.BirthDay);
            person.Phone = request.Phone;

            // Xóa các virus hiện tại của người dùng
            _context.PersonViruses.RemoveRange(person.PersonViruses);

            // Thêm các virus mới hoặc cập nhật nếu có
            foreach (var virusInfo in request.Viruses)
            {
                var virus = await _context.Viruses.FirstOrDefaultAsync(v => v.VirusName == virusInfo.VirusName);
                if (virus == null)
                {
                    virus = new Virus
                    {
                        VirusName = virusInfo.VirusName,
                        Treatment = null // Có thể để trống nếu không có thông tin Treatment
                    };
                    _context.Viruses.Add(virus);
                    await _context.SaveChangesAsync(); // Lưu lại để có VirusId
                }

                // Tạo đối tượng PersonVirus mới với virus và tỷ lệ kháng thuốc
                var personVirus = new PersonVirus
                {
                    PersonId = person.PersonId,
                    VirusId = virus.VirusId,
                    ResistanceRate = virusInfo.ResistanceRate
                };

                _context.PersonViruses.Add(personVirus);
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Person and viruses updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.People
                .Include(p => p.PersonViruses)
                .FirstOrDefaultAsync(p => p.PersonId == id);

            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            // Xóa các virus liên quan của người dùng
            _context.PersonViruses.RemoveRange(person.PersonViruses);

            // Xóa chính đối tượng Person
            _context.People.Remove(person);

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Person and related viruses deleted successfully" });
        }
    }
}
