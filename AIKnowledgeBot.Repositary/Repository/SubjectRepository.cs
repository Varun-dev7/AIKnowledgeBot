using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Repositary.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AIKnowledgeBot.Repositary.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects
                .AnyAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
