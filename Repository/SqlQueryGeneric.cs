using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LeaveManagment.Utils;

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

        public async Task<ICollection<T>> ExecuteSql(string query)
        {
            var collection = _db.FromSqlRaw(query);
            return await collection.ToListAsync();
        }
        public async Task<ICollection<T>> ExecuteSql(string query, SqlParameter singleParam)
        {
            var paramArray = new SqlParameter[] { };
            if (singleParam != null)
            {
                paramArray[0] = singleParam;
                return await _db.FromSqlRaw(query,paramArray).ToListAsync();
            }
            else
            {
                return await _db.FromSqlRaw(query).ToListAsync();
            }
            
        }
        public async Task<ICollection<T>> ExecuteSql(string query, object obj)
        {
           
            var collection = _db.FromSqlRaw(query.AddSqlParams(obj),obj.ProcedureMappingParams());
            return await collection.ToListAsync();
        }
    }
}
