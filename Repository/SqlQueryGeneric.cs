using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LeaveManagment.Repository
{
    public class SqlQueryGeneric<T> : ISqlQueryGeneric<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public SqlQueryGeneric(ApplicationDbContext context)
        {
            _context = context;
            _db = context.Set<T>();
        }

        public async Task<ICollection<T>> ExecuteSql(string sqlquery)
        {
            var obj = _db.FromSqlRaw(sqlquery);
            return await obj.ToListAsync();
        }

        public Task<T> ExecuteSql(string query, params SqlParameter[] parameters)
        {

            return null;
        }
    }
}
