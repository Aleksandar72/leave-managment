using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace LeaveManagment.Utils
{
    public static class CustomExtension
    {
        public static string AddSqlParams(this String str, object obj)
        {
            var props = obj.GetType().GetProperties();
            string propName = String.Empty;
            int counter = 0;
            foreach (var item in props)
            {
                
                propName = counter < 1 ? propName + " " + "@" + item.Name : propName + "," + "@" + item.Name;
                var pinfo = item.GetValue(obj,null);
                counter++;
            }
            
            return str + propName ;
        }
        public static SqlParameter[] ProcedureMappingParams(this object obj)
        {
            var props = obj.GetType().GetProperties();
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (var item in props)
            {
                bool param1 = item.PropertyType.Equals(typeof(DateTime?));
                string dateTimeToSql = String.Empty;
                if (item.PropertyType.Equals(typeof(DateTime?)))
                {
                    dateTimeToSql = ((DateTime)item.GetValue(obj)).ToString("yyyy-MM-dd HH:mm:ss");
                    list.Add(new SqlParameter("@" + item.Name.ToString(), dateTimeToSql));
                }
                else
                {
                    list.Add(new SqlParameter("@" + item.Name, item.GetValue(obj)));
                }
            }
            return list.ToArray();
        }
    }
}
