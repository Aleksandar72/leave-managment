using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Models
{
    public class LeaveAllocationVM
    {
      
        public string Id { get; set; }
        public int NumberOfDays { get; set; }
        public int Period { get; set; }
        public DateTime DateCreated { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public LeaveTypesVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
       
    }
    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }
        public int NumberOfDays { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public LeaveTypesVM LeaveType { get; set; }
    }
    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List<LeaveTypesVM> LeaveTypes { get; set; }
    }
    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }
}
