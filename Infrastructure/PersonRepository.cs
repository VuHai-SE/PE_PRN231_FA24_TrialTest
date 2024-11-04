using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<IList<Person>> SearchByKeywordAsync(string? keyword)
        {
            var query = Entities.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => EF.Functions.Like(p.Fullname, $"%{keyword}%") ||
                                         EF.Functions.Like(p.Phone, $"%{keyword}%"));
            }

            return await query
                .Include(p => p.PersonViruses)
                .ThenInclude(pv => pv.Virus)
                .ToListAsync();
        }
    }
}
