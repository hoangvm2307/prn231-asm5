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
    public class GrowthRecordRepository : GenericRepository<GrowthRecord>
    {
        public GrowthRecordRepository() { }

        public GrowthRecordRepository(ChildCheckerContext context) : base(context) { }

        public async Task<List<GrowthRecord>> GetAll()
        {
            var growthRecords = await _context.GrowthRecords.ToListAsync();
            return growthRecords;
        }

        public async Task<List<GrowthRecord>> GetByChildId(Guid childId)
        {
            return await _context.GrowthRecords
                .Where(g => g.ChildId == childId)
                .OrderByDescending(g => g.MeasurementDate)
                .ToListAsync();
        }

        public async Task<GrowthRecord> GetLatestByChildId(Guid childId)
        {
            return await _context.GrowthRecords
                .Where(g => g.ChildId == childId)
                .OrderByDescending(g => g.MeasurementDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GrowthRecord>> GetByDateRange(Guid childId, DateTime startDate, DateTime endDate)
        {
            return await _context.GrowthRecords
                .Where(g => g.ChildId == childId && g.MeasurementDate >= startDate && g.MeasurementDate <= endDate)
                .OrderByDescending(g => g.MeasurementDate)
                .ToListAsync();
        }

        // Thêm phương thức để lấy dữ liệu tăng trưởng cho biểu đồ
        public async Task<Dictionary<string, List<double>>> GetGrowthChartData(Guid childId)
        {
            var records = await _context.GrowthRecords
                .Where(g => g.ChildId == childId)
                .OrderBy(g => g.MeasurementDate)
                .ToListAsync();

            var result = new Dictionary<string, List<double>>
            {
                { "dates", records.Select(r => (double)(r.MeasurementDate.Ticks)).ToList() },
                { "weights", records.Select(r => (double)r.Weight).ToList() },
                { "heights", records.Select(r => (double)r.Height).ToList() },
                { "bmis", records.Select(r => (double)r.Bmi).ToList() }
            };

            // Thêm chu vi đầu nếu có dữ liệu
            if (records.Any(r => r.HeadCircumference.HasValue))
            {
                result.Add("headCircumferences", records.Select(r => (double)(r.HeadCircumference ?? 0)).ToList());
            }

            return result;
        }
    }
}