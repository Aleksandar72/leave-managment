using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Models
{
    public class LeaveTypesVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,35, ErrorMessage = "Set valid number")]
        public int DefaultDays { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
