using ChildTracking.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Repositories.DTO
{
    public class ChildViewModel
    {
        public Guid ChildId { get; set; }
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string MedicalConditions { get; set; }
        public string Allergies { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? SelectedGrowthRecordId { get; set; }
        public List<GrowthRecord> GrowthRecords { get; set; }
    }

}
