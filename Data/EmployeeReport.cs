using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Data
{
    public class EmployeeReport 
    {
        public string Name { get; set; }
        public int TotalLeaveDays { get; set; }
        public int TotalLeaveRequestDays { get; set; }
        public int TotalRequests { get; set; }
        public int TotalApproved { get; set; }
        public int TotalRejected { get; set; }
        public int TotalWaiting { get; set; }
       
    }
}
