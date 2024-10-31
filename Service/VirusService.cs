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
    public class VirusService : IVirusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VirusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetOrCreateVirusIdByNameAsync(string virusName)
        {
            var virusRepository = _unitOfWork.Repository<Virus>();

            // Check if the virus already exists by name
            var virus = await virusRepository.GetAll().FirstOrDefaultAsync(v => v.VirusName == virusName);
            if (virus != null)
            {
                return virus.VirusId;
            }

            // Find the highest virus_id and increment it for the new ID
            int newVirusId = (await virusRepository.GetAll().MaxAsync(v => (int?)v.VirusId) ?? 0) + 1;

            // Create and save the new virus with the calculated virus_id
            virus = new Virus
            {
                VirusId = newVirusId,
                VirusName = virusName,
                Treatment = null // Treatment can be set later if needed
            };
            await virusRepository.InsertAsync(virus, saveChanges: false);
            await _unitOfWork.SaveChangesAsync();

            return virus.VirusId;
        }
    }
}
