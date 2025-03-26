using ChildTracking.Repositories;
using ChildTracking.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Services
{
    public interface IChildService
    {
        Task<List<Child>> GetAll();
        Task<Child> GetById(Guid id);
        Task<int> Create(Child entity);
        Task<int> Update(Child entity);
        Task<bool> Delete(Guid id);
        Task<List<Child>> Search(string field1, string field2, string field3);
    }
    public class ChildService : IChildService
    {
        private readonly ChildRepository _repository;
        public ChildService()
        {
            _repository = new ChildRepository();
        }
        public async Task<int> Create(Child entity)
        {
            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> Delete(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item != null)
            {
                return await _repository.RemoveAsync(item);
            }

            return false;
        }

        public async Task<List<Child>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Child> GetById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Child>> Search(string field1, string field2, string field3)
        {
            return await _repository.Search(field1, field2, field3);
        }

        public async Task<int> Update(Child entity)
        {
            return await _repository.UpdateAsync(entity);
        }
    }
}
