using JobFitScoreAPI.Dtos.Vaga;
using JobFitScoreAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Services
{
    public interface IVagaService
    {
        Task<Vaga> CreateVagaAsync(VagaInput vagaDto, int empresaId);
        
        Task<VagaOutput?> GetVagaByIdAsync(int id); 
        Task<(IEnumerable<VagaOutput> vagas, int totalItems)> GetVagasAsync(
            string? termoBusca, int page, int pageSize); 
            
        Task<Vaga?> UpdateVagaAsync(int id, VagaUpdateInput vagaDto); 
        
        Task<bool> DeleteVagaAsync(int id);
    }
}