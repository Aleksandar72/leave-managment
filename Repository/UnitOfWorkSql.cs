using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Repository
{
    public class UnitOfWorkSql : IUnitOfWorkSql
    {
        private readonly ApplicationDbContext _context;
        private ISqlQueryGeneric<EmployeeReport> _employeeReport;
        private ISqlQueryGeneric<RequestStatistic> _requestReport;

        public UnitOfWorkSql(ApplicationDbContext context)
        {
            _context = context;
        }

        public ISqlQueryGeneric<EmployeeReport> EmployeeReport => _employeeReport ??= new SqlQueryGeneric<EmployeeReport>(_context);
        public ISqlQueryGeneric<RequestStatistic> RequestReport => _requestReport ??= new SqlQueryGeneric<RequestStatistic>(_context);
    }
}
