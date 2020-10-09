using LeaveManagment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<LeaveTypes> LeaveTypes { get; }
        IGenericRepository<LeaveRequest> LeaveRequests { get; }
        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }
        Task Save();
    }
}
