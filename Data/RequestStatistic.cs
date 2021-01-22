using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Data
{
    public class RequestStatistic
    {
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string RequestDate { get; set; }
        public string StartDate { get; set; }
        public string EndRequestDate { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

    }
}
