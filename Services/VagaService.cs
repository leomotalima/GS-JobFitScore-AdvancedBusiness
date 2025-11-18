using JobFitScoreAPI.Data;
using JobFitScoreAPI.Dtos.Vaga; 
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Services
{
    public class VagaService : IVagaService
    {
        private readonly AppDbContext _context;

        public VagaService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<Vaga> CreateVagaAsync(VagaInput vagaDto, int empresaId) 
        {
            var vaga = new Vaga
            {
                Titulo = vagaDto.Titulo,
                EmpresaId = empresaId 
            };
            
            await _context.Vagas.AddAsync(vaga);
            await _context.SaveChangesAsync();
            return vaga;
        }

        
        public async Task<VagaOutput?> GetVagaByIdAsync(int id)
        {
            return await _context.Vagas
                .Include(v => v.Empresa) 
                .Where(v => v.IdVaga == id)
                .Select(v => new VagaOutput
                {
                    IdVaga = v.IdVaga,
                    Titulo = v.Titulo,
                    EmpresaNome = v.Empresa!.Nome 
                })
                .FirstOrDefaultAsync();
        }

       
        public async Task<(IEnumerable<VagaOutput> vagas, int totalItems)> GetVagasAsync(
            string? termoBusca, int page, int pageSize) 
        {
            var query = _context.Vagas.Include(v => v.Empresa).AsQueryable();

            if (!string.IsNullOrEmpty(termoBusca))
            {
                query = query.Where(v => v.Titulo.Contains(termoBusca));
            }

            var totalItems = await query.CountAsync();

            var vagas = await query
                .OrderBy(v => v.Titulo) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VagaOutput 
                {
                    IdVaga = v.IdVaga,
                    Titulo = v.Titulo,
                    EmpresaNome = v.Empresa!.Nome, 
                })
                .ToListAsync();

            return (vagas, totalItems);
        }
        
        
        public async Task<Vaga?> UpdateVagaAsync(int id, VagaUpdateInput vagaDto) 
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return null;

            vaga.Titulo = vagaDto.Titulo ?? vaga.Titulo;

            await _context.SaveChangesAsync();
            return vaga;
        }

        
        public async Task<bool> DeleteVagaAsync(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null) return false;

            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}