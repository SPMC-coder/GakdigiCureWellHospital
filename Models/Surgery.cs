using System;
using System.ComponentModel.DataAnnotations;

namespace CureWellHospital.Models
{
    public class Surgery
    {
        // SurgeryId is the primary key and mandatory
        [Required]
        public int SurgeryId { get; set; }

        // DoctorId is mandatory
        [Required]
        public int DoctorId { get; set; }

        // SurgeryDate is mandatory
        [Required]
        public DateTime SurgeryDate { get; set; }

        // StartTime is mandatory and must be between 1 and 24
        [Required]
        [Range(1, 24, ErrorMessage = "StartTime must be between 1 and 24")]
        public int StartTime { get; set; }

        // EndTime is mandatory and must be between 1 and 24
        [Required]
        [Range(1, 24, ErrorMessage = "EndTime must be between 1 and 24")]
        public int EndTime { get; set; }

        // SurgeryCategory is mandatory and must have a length of exactly 3
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "SurgeryCategory must be 3 characters long")]
        public string SurgeryCategory { get; set; } = string.Empty;
    }
}
