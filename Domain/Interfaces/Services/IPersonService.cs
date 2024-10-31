using Domain.Entities;
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
        Task<Person?> GetPersonDetailsAsync(int id);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task AddPersonWithVirusesAsync(Person person, List<PersonVirus> personViruses);
        Task UpdatePersonAsync(Person person, List<PersonVirus> personViruses);
        Task DeletePersonAsync(int id);
        //Task<List<PersonVirus>> PreparePersonVirusesAsync(int personId, List<string> virusNames, List<float> resistanceRates);
    }
}
