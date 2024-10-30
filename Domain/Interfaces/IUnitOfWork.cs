using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Person> PersonRepository { get; }
        IGenericRepository<ViroCureUser> ViroCureUserRepository { get; }
        IGenericRepository<Virus> VirusRepository { get; }
        IGenericRepository<PersonVirus> PersonVirusRepository { get; }

        Task SaveAsync();
    }
}
