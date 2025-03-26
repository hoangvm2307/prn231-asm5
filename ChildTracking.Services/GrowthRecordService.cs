using ChildTracking.Repositories;
using ChildTracking.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Services
{
    public class GrowthRecordService
    {
        private readonly GrowthRecordRepository _repository;

        public GrowthRecordService()
        {
            _repository = new GrowthRecordRepository();
        }

        public async Task<List<GrowthRecord>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<GrowthRecord> GetById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<GrowthRecord>> GetByChildId(Guid childId)
        {
            return await _repository.GetByChildId(childId);
        }

        public async Task<int> Create(GrowthRecord record)
        {
            if (record.RecordId == Guid.Empty)
            {
                record.RecordId = Guid.NewGuid();
            }
            
            if (record.CreatedAt == DateTime.MinValue)
            {
                record.CreatedAt = DateTime.Now;
            }

            // Calculate BMI if weight and height are provided but BMI is not
            if (record.Weight > 0 && record.Height > 0 && record.Bmi <= 0)
            {
                // Chuyển đổi từ decimal? sang double
                double height = (double)record.Height.Value;
                double weight = (double)record.Weight.Value;

                // Tính toán BMI
                double heightInMeters = height / 100.0;
                record.Bmi = (decimal)Math.Round(weight / (heightInMeters * heightInMeters), 2);
            }

            return await _repository.CreateAsync(record);
        }

        public async Task<int> Update(GrowthRecord record)
        {
            // Calculate BMI if weight and height are provided
            if (record.Weight > 0 && record.Height > 0)
            {
                double height = (double)record.Height.Value;
                double weight = (double)record.Weight.Value;

                // Tính toán BMI
                double heightInMeters = height / 100.0;
                record.Bmi = (decimal)Math.Round(weight / (heightInMeters * heightInMeters), 2);
            }

            return await _repository.UpdateAsync(record);
        }

        public async Task<bool> Delete(Guid id)
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
            {
                return false;
            }

            return await _repository.RemoveAsync(record);
        }
    }
}