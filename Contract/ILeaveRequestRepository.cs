using LeaveManagment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
    public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
    {
      Task<ICollection<LeaveRequest>> GetLeaveRequestByEmployeeId(string employeeId); 
    }
}
