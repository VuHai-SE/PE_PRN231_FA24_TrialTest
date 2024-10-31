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
    public class ViroCureUserService : IViroCureUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ViroCureUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ViroCureUser?> Login(string email, string password)
        {
            var userRepository = _unitOfWork.Repository<ViroCureUser>();
            var user = await userRepository.GetAll().FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            return user;
        }

        //public async Task<ViroCureUser?> GetByVerificationToken(string token)
        //{
        //    var userRepository = _unitOfWork.Repository<ViroCureUser>();
        //    return await userRepository.GetAll().AsNoTracking()
        //        .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        //}

        //public async Task Update(ViroCureUser user)
        //{
        //    var userRepository = _unitOfWork.Repository<ViroCureUser>();
        //    await _unitOfWork.BeginTransaction();
        //    await userRepository.UpdateAsync(user);
        //    await _unitOfWork.CommitTransaction();
        //}

        //public async Task Add(ViroCureUser user)
        //{
        //    var userRepository = _unitOfWork.Repository<ViroCureUser>();
        //    await _unitOfWork.BeginTransaction();
        //    await userRepository.InsertAsync(user);
        //    await _unitOfWork.CommitTransaction();
        //}
    }
}
