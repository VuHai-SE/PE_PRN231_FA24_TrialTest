using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ViroCureFal2024dbContext _context;
        private IGenericRepository<Person> _personRepository;
        private IGenericRepository<ViroCureUser> _viroCureUserRepository;
        private IGenericRepository<Virus> _virusRepository;
        private IGenericRepository<PersonVirus> _personVirusRepository;

        public UnitOfWork(ViroCureFal2024dbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Person> PersonRepository => _personRepository ??= new GenericRepository<Person>(_context);
        public IGenericRepository<ViroCureUser> ViroCureUserRepository => _viroCureUserRepository ??= new GenericRepository<ViroCureUser>(_context);
        public IGenericRepository<Virus> VirusRepository => _virusRepository ??= new GenericRepository<Virus>(_context);
        public IGenericRepository<PersonVirus> PersonVirusRepository => _personVirusRepository ??= new GenericRepository<PersonVirus>(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
