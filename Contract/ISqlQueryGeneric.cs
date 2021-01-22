using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
    public interface ISqlQueryGeneric<T> where T : class
    {
        Task<ICollection<T>> ExecuteSql(string query);
        Task<ICollection<T>> ExecuteSql(string query, Object obj);
        Task<ICollection<T>> ExecuteSql(string query, SqlParameter singleParam);


    }
}
