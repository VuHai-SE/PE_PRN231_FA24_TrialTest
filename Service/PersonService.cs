using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Person?> GetPersonDetailsAsync(int id)
        {
            var personRepository = _unitOfWork.Repository<Person>();
            return await personRepository.GetAll()
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .FirstOrDefaultAsync(p => p.PersonId == id);
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            var personRepository = _unitOfWork.Repository<Person>();
            return await personRepository.GetAll()
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .ToListAsync();
        }

        public async Task AddPersonWithVirusesAsync(Person person, List<PersonVirus> personViruses)
        {
            await _unitOfWork.BeginTransaction();
            await _unitOfWork.Repository<Person>().InsertAsync(person, false);
            await _unitOfWork.Repository<PersonVirus>().InsertRangeAsync(personViruses, false);
            await _unitOfWork.CommitTransaction();
        }

        public async Task UpdatePersonAsync(Person person, List<PersonVirus> personViruses)
        {
            await _unitOfWork.BeginTransaction();
            var personRepository = _unitOfWork.Repository<Person>();
            var personVirusRepository = _unitOfWork.Repository<PersonVirus>();

            // Update Person and related PersonViruses
            await personRepository.UpdateAsync(person, false);
            await personVirusRepository.DeleteRangeAsync(person.PersonViruses, false);  // Remove existing viruses
            await personVirusRepository.InsertRangeAsync(personViruses, false);         // Insert updated viruses
            await _unitOfWork.CommitTransaction();
        }

        public async Task DeletePersonAsync(int id)
        {
            var personRepository = _unitOfWork.Repository<Person>();
            var personVirusRepository = _unitOfWork.Repository<PersonVirus>();

            var person = await personRepository.FindAsync(id);
            if (person != null)
            {
                await _unitOfWork.BeginTransaction();
                await personVirusRepository.DeleteRangeAsync(person.PersonViruses, false);
                await personRepository.DeleteAsync(person, false);
                await _unitOfWork.CommitTransaction();
            }
        }
    }
}
