using ChildTracking.Repositories.Base;
using ChildTracking.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Repositories
{
    public class ChildRepository : GenericRepository<Child>
    {
        public ChildRepository() { }
        public async Task<List<Child>> GetAll()
        {
            var objects = await _context.Children.Include(b => b.GrowthRecords).ToListAsync();
            return objects;
        }
        public async Task<Child> GetByIdAsync(Guid code)
        {
            var entity = await _context.Set<Child>().Include(c => c.GrowthRecords).FirstOrDefaultAsync(c => c.ChildId.Equals(code));
           
            return entity;
        }
        public async Task<List<Child>> Search(string field1, string field2, string field3)
        {
            var objects = await _context.Children
                .Include(c => c.GrowthRecords)
                .Where(c =>
                    (c.FullName.Contains(field1) || string.IsNullOrEmpty(field1))
                    && (c.BloodType.Contains(field2) || string.IsNullOrEmpty(field2))
                    && (c.MedicalConditions.Contains(field3) || string.IsNullOrEmpty(field3))).ToListAsync();

            return objects;
        }
    }
}
