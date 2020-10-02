using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
     public interface IRepositoryBase<T> where T : class
    {
        ICollection<T> FindAll();
        bool isExist(int Id); 
        T FindById(int Id);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Save();
    }
}
