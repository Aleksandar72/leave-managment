using LeaveManagment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
    public interface IUnitOfWorkSql 
    {
        ISqlQueryGeneric<EmployeeReport> EmployeeReport { get; }
        ISqlQueryGeneric<RequestStatistic> RequestReport { get; }
    }
}
