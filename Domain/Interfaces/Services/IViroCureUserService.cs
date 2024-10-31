using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IViroCureUserService
    {
        Task<ViroCureUser?> Login(string email, string password);
        //Task<ViroCureUser?> GetByVerificationToken(string token);
        //Task Update(ViroCureUser user);
        //Task Add(ViroCureUser user);
    }
}
